using System.IdentityModel.Tokens.Jwt;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FreeCourse.Shared.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, string serverUrl, string audience)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = serverUrl;
                options.Audience = audience;
                options.RequireHttpsMetadata = false;
            });

            return services;
        }

        public static IServiceCollection ConfigureSharedIdentityService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();//SharedIdentityService>IHttpContextAccessor
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();

            return services;
        }

        public static IServiceCollection RemoveJwtDefaultClaimTypeMap(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            return services;
        }

        public static IServiceCollection AddAuthorizeFilterToControllersWithPolicy(this IServiceCollection services)
        {
            //Tüm controller'larda AuthorizeFilter tanımlanması için
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            return services;
        }

        public static IServiceCollection AddAuthorizeFilterToControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });

            return services;
        }
    }
}
