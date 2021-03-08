using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Data;
using Web.Services;
using Web.Services.SupportRequestRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportRequestController : ControllerBase
    {
        private readonly ISupportRequestRepository _repo;
        private readonly SupportRequestManager _manager;

        public SupportRequestController(ISupportRequestRepository repo, SupportRequestManager manager)
        {
            _repo = repo;
            _manager = manager;
        }

        /// <summary>
        /// Return all support requests
        /// </summary>
        /// <returns>All support requests</returns>
        [HttpGet]
        public async Task<IList<SupportRequest>> Get()
        {
            return await _repo.GetAll();
        }

        /// <summary>
        /// Return support request by id
        /// </summary>
        /// <returns>support request by id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _repo.Get(id);
            return result != null ? new OkObjectResult(result) : (IActionResult) new BadRequestResult();
        }

        /// <summary>
        /// Creates a support request
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "head": "title of request",
        ///         "text": "request text"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">support request for create</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SupportRequest request)
        {
            var result = await _manager.AddRequest(request);
            return result ? new OkResult() : (IActionResult) new BadRequestResult();
        }

        /// <summary>
        /// Decline support request by id
        /// </summary>
        /// <param name="id">id support request for decline</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Decline(int id)
        {
            var result = await _manager.DeclineRequest(id);
            return result ? new OkResult() : (IActionResult) new BadRequestResult();
        }
    }
}