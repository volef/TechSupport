using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.Dev;
using Web.Data.SupportIdentity;
using Web.Services;
using Web.Services.SupportIdentityManagers;
using Web.Services.SupportRequestRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Controllers
{
    [Route("dev/[action]")]
    [ApiController]
    public class DevController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ISupportRequestRepository _repo;
        private readonly ISupportIdentityManager _supportIdentityManager;

        public DevController(UserManager<AppUser> userManager, ISupportRequestRepository repo,
            ISupportIdentityManager supportIdentityManager)
        {
            _userManager = userManager;
            _repo = repo;
            _supportIdentityManager = supportIdentityManager;
        }

        /// <summary>
        /// Intitate 3 users in support team (director,manager,operator) with passwords (directorpwd,managerpwd,operatorpwd)
        /// Add 3 support request
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> InitTestData()
        {
            //
            var directorUser = new AppUser {UserName = "director"};
            var result = await _userManager.CreateAsync(directorUser, "directorpwd");
            await _supportIdentityManager.AddUserToSupport(directorUser, "director");
            //
            var managerUser = new AppUser {UserName = "manager"};
            await _userManager.CreateAsync(managerUser, "managerpwd");
            var managerIdentity = await _supportIdentityManager.AddUserToSupport(managerUser, "manager");
            //
            var operatorUser = new AppUser {UserName = "operator"};
            await _userManager.CreateAsync(operatorUser, "operatorpwd");
            await _supportIdentityManager.AddUserToSupport(operatorUser, "operator");
            //
            await _repo.Create(new SupportRequest
            {
                Head = "First", Text = "First text", Created = DateTime.Now, User = operatorUser,
                State = SupportRequestState.IsDeclined
            });
            //
            var secondRequest = new SupportRequest
            {
                Head = "Second", Text = "Second text", Created = DateTime.Now, Support = managerUser,
                State = SupportRequestState.InProcessing
            };
            await _repo.Create(secondRequest);
            managerIdentity.State = SupportIdentityState.Working;
            managerIdentity.CurrentRequest = secondRequest;
            _supportIdentityManager.Update(managerIdentity);
            //
            await _repo.Create(new SupportRequest
            {
                Head = "Third", Text = "Third text", Created = DateTime.Now, Support = directorUser,
                State = SupportRequestState.IsDone, DoneTime = DateTime.Now.AddMinutes(5d)
            });
            //
            await _repo.Create(new SupportRequest {Head = "Fourth", Text = "Fourth text", Created = DateTime.Now});
            //
            return new OkResult();
        }

        /// <summary>
        /// Creates user and add to support team
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUserModel model)
        {
            var User = new AppUser {UserName = model.Username};
            var result = await _userManager.CreateAsync(User, model.Password);
            if (!result.Succeeded)
                return new BadRequestResult();
            await _supportIdentityManager.AddUserToSupport(User, model.Role);
            return new OkResult();
        }
    }
}