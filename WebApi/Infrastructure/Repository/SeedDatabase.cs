using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Model;

namespace WebApi.Infrastructure.Repository
{
    public class SeedDatabase
    {
        private readonly AppDbContext _dataContext;

        public SeedDatabase(AppDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckData();
        }

        private async Task CheckData()
        {
            await SaveData<Store>("store");
            await SaveData<Product>("product");
        }

        private async Task SaveData<T>(string name) where T : AuditableEntity
        {
            if (_dataContext.Set<T>().Any())
            {
                return;
            }
            string json = File.ReadAllText($"data/{name}.json");
            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
            list.ForEach(SetData);
            await _dataContext.SaveChangesAsync();
        }

        private void SetData<T>(T data) where T : AuditableEntity
        {
            _dataContext.Set<T>().Add(data);
        }
    }
}
