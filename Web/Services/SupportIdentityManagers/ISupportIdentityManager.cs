using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.SupportIdentity;

namespace Web.Services.SupportIdentityManagers
{
    public interface ISupportIdentityManager<TSupportIdentity, TUser>
        where TUser : AppUser
        where TSupportIdentity : class, ISupportIdentity<TUser>, new()
    {
        public Task<TSupportIdentity> Get(TUser user);
        public Task<List<TSupportIdentity>> GetAll();
        public Task<TSupportIdentity> AddUserToSupport(TUser user, string roleName);
        public TSupportIdentity Update(TSupportIdentity identity);
    }

    public interface ISupportIdentityManager : ISupportIdentityManager<SupportIdentity, AppUser>
    {
    }
}