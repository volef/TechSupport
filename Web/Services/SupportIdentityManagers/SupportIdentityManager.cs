using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.SupportIdentity;

namespace Web.Services.SupportIdentityManagers
{
    public class SupportIdentityManager : ISupportIdentityManager
    {
        private readonly ILogger<SupportIdentityManager> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public SupportIdentityManager(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager, ILogger<SupportIdentityManager> logger)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<SupportIdentity> Get(AppUser user)
        {
            return await _dbContext.SupportIdentities
                .Include(identity => identity.Owner)
                .Include(identity => identity.CurrentRequest)
                .FirstOrDefaultAsync(identity => identity.Owner == user);
        }

        public async Task<List<SupportIdentity>> GetAll()
        {
            return await _dbContext.SupportIdentities
                .Include(identity => identity.Owner)
                .Include(identity => identity.CurrentRequest)
                .ToListAsync();
        }

        public async Task<SupportIdentity> AddUserToSupport(AppUser user, string roleName)
        {
            var get = await _dbContext.SupportIdentities
                .FirstOrDefaultAsync(identity => identity.Owner == user);
            if (get != null)
                return get;
            var newIdentityEntry = await _dbContext.SupportIdentities
                .AddAsync(new SupportIdentity {Owner = user});
            _dbContext.SaveChanges();
            // добавляем роль юзеру
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            await _userManager.AddToRoleAsync(user, roleName);
            //
            _logger.LogInformation($"Added user{user.Id} to support team identity{newIdentityEntry.Entity.Id}");
            return newIdentityEntry.Entity;
        }

        public SupportIdentity Update(SupportIdentity identity)
        {
            var updatedIdentityEntry = _dbContext.SupportIdentities.Update(identity);
            _dbContext.SaveChanges();
            _logger.LogInformation($"Support identity{updatedIdentityEntry.Entity.Id} updated");
            return updatedIdentityEntry.Entity;
        }
    }
}