using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.SupportIdentity;
using Web.Services.SupportIdentityManagers;
using Web.Services.SupportRequestRepository;

namespace Web.Services
{
    public class SupportRequestManager
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SupportRequestManager> _logger;
        private readonly ISupportRequestRepository _repo;
        private readonly SupportRequestQueue.SupportRequestQueue _queue;
        private readonly ISupportIdentityManager _supportIdentityManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public SupportRequestManager(ISupportRequestRepository repo,
            IConfiguration configuration,
            SupportRequestQueue.SupportRequestQueue queue,
            ISupportIdentityManager supportIdentityManager,
            IHttpContextAccessor httpContext,
            UserManager<AppUser> userManager, ILogger<SupportRequestManager> logger)
        {
            _repo = repo;
            _config = configuration;
            _queue = queue;
            _supportIdentityManager = supportIdentityManager;
            _httpContext = httpContext;
            _userManager = userManager;

            if (!_queue.IsInitialized)
            {
                _queue.IsInitialized = true;
                foreach (var request in _repo.GetNotProcessed()) _queue.Enqueue(request);
                ;
            }

            _logger = logger;
        }

        public async Task<bool> AddRequest(SupportRequest request)
        {
            var result = await _repo.Create(request);
            if (result == null)
                return false;
            _queue.Enqueue(request);
            _logger.LogInformation($"Added support request id{request.Id}");
            return true;
        }

        public async Task<bool> DeclineRequest(int id)
        {
            var result = await _repo.Decline(id);
            if (result)
                _logger.LogInformation($"Support request id{id} declined");
            else
                _logger.LogWarning($"Support request id{id} error decline");
            return result;
        }

        public async Task<SupportRequest> GetNewRequest()
        {
            //дополнительная проверка что мы не перезапишем открытый запрос
            var current = await GetCurrentRequest();
            if (current != null)
                return current;
            //
            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("operator"))
                return await GetNewRequest(_config.GetValue<int>("T"));
            else if (roles.Contains("manager"))
                return await GetNewRequest(_config.GetValue<int>("Tm"));
            else if (roles.Contains("director"))
                return await GetNewRequest(_config.GetValue<int>("Td"));
            else
                _logger.LogWarning($"User without support role use GetNewRequest()");
            return null;
        }

        private async Task<SupportRequest> GetNewRequest(int sleepTime)
        {
            SupportRequest request = null;
            do
            {
                await Task.Delay(1000);
                while (!_queue.HasRequest())
                    await Task.Delay(sleepTime);
                if (_queue.CanProcessRequest(sleepTime))
                {
                    request = _queue.Dequeue();
                    // проверяем что запрос не отменил юзер
                    request = await _repo.Get(request.Id);
                    if (request.State == SupportRequestState.IsDeclined)
                        request = null;
                }
            } while (request == null);

            //вытаскиваем юзера из контекста
            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            //обновляем текущий запрос и статус в карте поддержки
            var identity = await _supportIdentityManager.Get(user);
            identity.CurrentRequest = request;
            identity.State = SupportIdentityState.Working;
            _supportIdentityManager.Update(identity);
            //обновляем статус самого запроса
            request.State = SupportRequestState.InProcessing;
            request.Support = user;
            _repo.Update(request);
            _logger.LogInformation($"Support id{identity.OwnerId} took request id{request.Id}");
            //
            return request;
        }

        public async Task<SupportRequest> GetCurrentRequest()
        {
            //вытаскиваем юзера из контекста
            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            //достаем текущий запрос из карты поддержки
            var identity = await _supportIdentityManager.Get(user);
            return identity.CurrentRequest;
        }

        public async Task DoneRequest(int id)
        {
            var request = await _repo.Get(id);
            //обновляем статус запроса
            request.State = SupportRequestState.IsDone;
            request.DoneTime = DateTime.Now;
            _repo.Update(request);
            //вытаскиваем юзера из контекста
            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            //обновляем текущий запрос и статус в карте поддержки
            var identity = await _supportIdentityManager.Get(user);
            identity.CurrentRequest = null;
            identity.State = SupportIdentityState.Ready;
            _supportIdentityManager.Update(identity);
            _logger.LogInformation($"Support id{identity.OwnerId} done request id{request.Id}");
        }
    }
}