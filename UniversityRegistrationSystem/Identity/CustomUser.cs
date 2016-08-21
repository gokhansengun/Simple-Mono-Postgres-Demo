using Microsoft.AspNet.Identity;
using UniversityRegistrationSystem.OwinConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace UniversityRegistrationSystem.Identity
{
    public class CustomUser : IUser<string>
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Id { get; set; }

        public bool EmailConfirmed { get; internal set; }

        public bool PhoneNumberConfirmed { get; internal set; }

        public CustomUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        // ** Add authenticationtype as method parameter:
        public async Task<ClaimsIdentity>
            GenerateUserIdentityAsync(ApplicationUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}