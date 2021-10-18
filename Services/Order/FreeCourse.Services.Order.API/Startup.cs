using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FreeCourse.Services.Order.API
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
            services.ConfigureAuthentication(Configuration["IdentityServerURL"], "resource_order");
            services.AddAuthorizeFilterToControllersWithPolicy();
            services.ConfigureSharedIdentityService();
            services.RemoveJwtDefaultClaimTypeMap();

            ConfigureSqlServer(services);
            ConfigureMediatR(services);


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Order.API", Version = "v1" });
            });
        }

        private void ConfigureMediatR(IServiceCollection services)
        {
            services.AddMediatR(typeof(Application.Handlers.CreateOrderCommandHandler).Assembly);
        }

        private void ConfigureSqlServer(IServiceCollection services)
        {
            services.AddDbContext<OrderDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), optBuilder =>
                {
                    optBuilder.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Order.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
