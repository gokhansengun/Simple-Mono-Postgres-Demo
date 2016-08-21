using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Dapper;

namespace UniversityRegistrationSystem.Identity
{
    public partial class CustomUserStore<TUser> : IUserPasswordStore<TUser> where TUser : CustomUser
    {
        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            { 
                throw new ArgumentNullException("user");
            }

            // TODO: gseng - correctly hash here
            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            { 
                throw new ArgumentNullException("user");
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }
    }
}