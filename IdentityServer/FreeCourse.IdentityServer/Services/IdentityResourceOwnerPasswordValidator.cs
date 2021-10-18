using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeCourse.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace FreeCourse.IdentityServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var existUser = await _userManager.FindByEmailAsync(context.UserName);

            if (existUser == null)
            {
                context.Result.CustomResponse = GetValidationError();
                return;
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);

            if (!passwordCheck)
            {
                context.Result.CustomResponse = GetValidationError();
                return;
            }

            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }

        private Dictionary<string, object> GetValidationError()
            => new Dictionary<string, object>
            {
                    { "errors", new List<string> { "Email veya şifreniz yanlış" } }
                };
    }
}
