using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<SupportRequest> SupportRequests { get; set; }
        public DbSet<SupportIdentity.SupportIdentity> SupportIdentities { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}