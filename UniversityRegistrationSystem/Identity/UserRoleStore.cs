using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using UniversityRegistrationSystem.DataAccess;
using UniversityRegistrationSystem.Utility;

namespace UniversityRegistrationSystem.Identity
{
    public partial class CustomUserStore<TUser> : IUserRoleStore<TUser> where TUser : CustomUser
    {
        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<string> roles = null;

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                roles = connection.Query<string>(FileHandler.ReadFileContent("Roles.GetAllRoleNamesByUserId.sql"),
                    new { Id = user.Id }).ToList();
            }

            if (roles != null)
            {
                return Task.FromResult<IList<string>>(roles);
            }
            
            return Task.FromResult<IList<string>>(null);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}