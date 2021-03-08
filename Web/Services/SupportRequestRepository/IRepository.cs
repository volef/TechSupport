using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Services.SupportRequestRepository
{
    public interface IRepository<TValue, TKey>
    {
        public Task<TValue> Create(TValue value);
        public Task<TValue> Get(TKey id);
        public Task<List<TValue>> GetAll();
        public Task<bool> Delete(TKey id);
        public TValue Update(TValue request);
    }
}