using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Data;

namespace Web.Services.SupportRequestRepository
{
    public interface ISupportRequestRepository : IRepository<SupportRequest, int>
    {
        public Task<bool> Decline(int id);
        public List<SupportRequest> GetNotProcessed();
    }
}