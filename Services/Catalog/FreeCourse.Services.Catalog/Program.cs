using System.Linq;
using FreeCourse.Services.Catalog.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FreeCourse.Services.Catalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var categoryService = serviceProvider.GetRequiredService<ICategoryService>();

                if (!categoryService.GetAllAsync().Result.Data.Any())
                {
                    categoryService.CreateAsync(new Dtos.CategoryCreateDto() { Name = ".Net", }).Wait();
                    categoryService.CreateAsync(new Dtos.CategoryCreateDto() { Name = "SQL", }).Wait();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
