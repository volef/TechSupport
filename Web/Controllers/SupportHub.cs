using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Web.Data;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = "director,manager,operator")]
    public class SupportHub : Hub
    {
        private readonly SupportRequestManager _manager;

        public SupportHub(SupportRequestManager manager)
        {
            _manager = manager;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveRequest", new SupportRequest {Id = 0, Head = "Тестовый запрос"});
        }

        public async Task GetCurrentRequest()
        {
            var current = await _manager.GetCurrentRequest();
            if (current != null)
                await Clients.Caller.SendAsync("ReceiveRequest", current);
        }

        public async Task GetRequest()
        {
            await Clients.Caller.SendAsync("ReceiveRequest", await _manager.GetNewRequest());
        }

        public async Task SendDoneRequest(int id)
        {
            await _manager.DoneRequest(id);
        }
    }
}