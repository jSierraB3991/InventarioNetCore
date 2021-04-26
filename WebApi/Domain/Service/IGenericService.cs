using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Domain.Model;

namespace WebApi.Domain.Service
{
    public interface IGenericService<Entity> where Entity : AuditableEntity
    {
        Task<List<Entity>> GetListAsync();

        Task<Entity> FindByIdAllData(System.Guid id);

        Task<Entity> GetByIdAsync(System.Guid id);

        Task<Entity> SaveAsync(Entity entity);

        Task<Entity> UpdateAsync(System.Guid id, Entity entity);

        Task DeleteByidAync(System.Guid id);
    }
}
