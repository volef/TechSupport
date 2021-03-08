using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Data;

namespace Web.Services.SupportRequestRepository
{
    public class SupportRequestRepository : ISupportRequestRepository
    {
        private readonly ILogger<SupportRequestRepository> _logger;
        private readonly ApplicationDbContext dbContext;

        public SupportRequestRepository(ApplicationDbContext dbContext, ILogger<SupportRequestRepository> logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task<SupportRequest> Create(SupportRequest request)
        {
            request.Created = DateTime.Now;
            var result = await dbContext.SupportRequests.AddAsync(request);
            dbContext.SaveChanges();
            _logger.LogInformation($"Support request{result.Entity.Id} created");
            return result.Entity;
        }

        public async Task<SupportRequest> Get(int id)
        {
            var result = await dbContext.SupportRequests
                .Include(request => request.Support)
                .Include(request => request.User)
                .FirstOrDefaultAsync(request => request.Id == id);
            return result;
        }

        public async Task<List<SupportRequest>> GetAll()
        {
            return await dbContext.SupportRequests
                .Include(request => request.Support)
                .Include(request => request.User)
                .ToListAsync();
        }

        public async Task<bool> Delete(int id)
        {
            return await Decline(id);
        }

        public async Task<bool> Decline(int id)
        {
            var result = await dbContext.SupportRequests.FindAsync(id);
            if (result != null)
            {
                result.State = SupportRequestState.IsDeclined;
                dbContext.SaveChanges();
                _logger.LogInformation($"Support request{id} declined");
            }
            else
            {
                _logger.LogInformation($"Support request{id} already declined");
            }

            return true;
        }

        public SupportRequest Update(SupportRequest request)
        {
            var result = dbContext.SupportRequests.Update(request);
            dbContext.SaveChanges();
            _logger.LogInformation($"Support request{request.Id} updated");
            return result.Entity;
        }

        public List<SupportRequest> GetNotProcessed()
        {
            return dbContext.SupportRequests
                .Include(request => request.Support)
                .Include(request => request.User)
                .Where(request => request.State == SupportRequestState.InQueue)
                .ToList();
        }
    }
}