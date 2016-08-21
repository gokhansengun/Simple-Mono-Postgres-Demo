using UniversityRegistrationSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Web;
using UniversityRegistrationSystem.Identity;
using System.Collections.Generic;

namespace UniversityRegistrationSystem.OwinConfig
{
    public class ApplicationUserManager 
        : UserManager<CustomUser>
    {
        public ApplicationUserManager(IUserStore<CustomUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options, 
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new CustomUserStore<CustomUser>());

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<CustomUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
				manager.UserTokenProvider = 
				    new DataProtectorTokenProvider<CustomUser>(
				        dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }

    public class ApplicationRoleManager : RoleManager<CustomRole>
    {
        public ApplicationRoleManager(IRoleStore<CustomRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options, 
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new CustomRoleStore<CustomRole>());
        }
    }
}
