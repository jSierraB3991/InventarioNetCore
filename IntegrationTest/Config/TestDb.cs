using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Repository;

namespace IntegrationTest.Config
{
    public class TestDb
    {
        private readonly AppDbContext dbContext;

        public TestDb()
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase(databaseName: "AppDbContext");

            dbContext = new AppDbContext(builder.Options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        public static AppDbContext GetContext() 
        {
            var test = new TestDb();
            return test.dbContext;
        }
    }
}
