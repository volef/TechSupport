using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Services;
using Web.Services.SupportIdentityManagers;
using Web.Services.SupportRequestRepository;

namespace Web.Controllers
{
    [Authorize(Roles = "director,manager,operator")]
    public class SupportController : Controller
    {
        private readonly ISupportRequestRepository _repo;
        private readonly ISupportIdentityManager _supportIdentityManager;

        public SupportController(ISupportRequestRepository repo,
            ISupportIdentityManager supportIdentityManager)
        {
            _repo = repo;
            _supportIdentityManager = supportIdentityManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> State()
        {
            return View(await _supportIdentityManager.GetAll());
        }

        public async Task<IActionResult> Queue()
        {
            return View(await _repo.GetAll());
        }
    }
}