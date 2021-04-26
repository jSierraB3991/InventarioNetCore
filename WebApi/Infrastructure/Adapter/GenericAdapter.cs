using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Domain.Exceotion;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Repository;

namespace WebApi.Infrastructure.Adapter
{
    public abstract class GenericAdapter<Entity> : IGenericService<Entity> where Entity : AuditableEntity
    {
        private readonly AppDbContext context;
        protected DbSet<Entity> DbEntity;
        public virtual string Name { get; }

        public GenericAdapter(AppDbContext context)
        {
            this.context = context;
            DbEntity = context.Set<Entity>();
        }


        public async Task DeleteByidAync(Guid id)
        {
            var exists = FindById(id);
            if (exists == null)
            {
                throw new BusinessException($"No exists {Name} With id {id}");
            }
            DbEntity.Remove(exists);
            await SaveAsync();
        }

        public async Task<Entity> GetByIdAsync(Guid id)
        {
            var exists = await DbEntity.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id));
            if (exists == null)
            {
                throw new BusinessException($"No exists {Name} With id {id}");
            }
            return exists;
        }

        public Task<List<Entity>> GetListAsync()
        {
            return DbEntity.ToListAsync();
        }

        public async Task<Entity> SaveAsync(Entity entity)
        {
            var exists = FindById(entity.Id);
            if (exists != null)
            {
                return exists;
            }

            try
            {
                entity.Id = Guid.NewGuid();
                context.Entry(entity).State = EntityState.Added;
                DbEntity.Add(entity);
                await SaveAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                await VerifyData(entity);
                throw new Exception(ex.Message);
            }
        }

        public void SaveNoSafe(Entity entity)
        {
            var exists = FindById(entity.Id);
            if (exists != null)
            {
                return;
            }

            entity.Id = Guid.NewGuid();
            context.Entry(entity).State = EntityState.Added;
            DbEntity.Add(entity);
        }

        public async Task<Entity> UpdateAsync(Guid id, Entity entity)
        {
            try
            {
                await GetByIdAsync(id);
                entity.Id = id;
                context.Entry(entity).State = EntityState.Modified;
                await SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                await VerifyData(entity);
                throw new Exception(ex.Message);
            }
            return entity;
        }

        public Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        private Entity FindById(Guid id)
        {
            if (!string.IsNullOrWhiteSpace(id.ToString()))
            {
                var exists = DbEntity.Find(id);
                if (exists != null)
                {
                    return exists;
                }
            }
            return null;
        }

        public virtual Task<Entity> FindByIdAllData(Guid id)
        {
            return GetByIdAsync(id);
        }

        public virtual async Task VerifyData(Entity entity)
        {
            await Task.Delay(5);
        }
    }
}
