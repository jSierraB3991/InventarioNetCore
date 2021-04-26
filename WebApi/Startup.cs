using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Adapter;
using WebApi.Infrastructure.Repository;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });

            services.AddTransient<SeedDatabase>();
            services.AddSingleton<ILogger>(options =>
            {
                string connectionString = Configuration["Serilog:DefaultConection"];
                string table = Configuration["Serilog:Table"];

                return new LoggerConfiguration()
                    .WriteTo
                    .MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = table, AutoCreateSqlTable = true })
                    .CreateLogger();
            });



            //SqlServer Connection
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ApiContext")));

            //Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Service
            services.AddTransient<IProductService, ProductAdapter>();
            services.AddTransient<IStoreService, StoreAdapter>();
            services.AddTransient<IMovementService, MovementAdapter>();
            services.AddTransient<IProductStoreService, ProductStoreAdapter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options =>
            {
                options.WithOrigins("http://localhost:3000");
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
