using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Dapper;
using UniversityRegistrationSystem.DataAccess;
using UniversityRegistrationSystem.Utility;

namespace UniversityRegistrationSystem.Identity
{
    public partial class CustomUserStore<TUser> : IUserLoginStore<TUser> where TUser : CustomUser
    {
        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            { 
                throw new ArgumentNullException("user");
            }

            if (login == null)
            { 
                throw new ArgumentNullException("login");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                connection.Execute(FileHandler.ReadFileContent("UserLogin.Create.sql"),
                    new { userId = user.Id, loginProvider = login.LoginProvider, providerKey = login.ProviderKey });
            }

            return Task.FromResult(0);
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            { 
                throw new ArgumentNullException("login");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                return Task.FromResult(connection.Query<TUser>(FileHandler.ReadFileContent("User.FindByProviderAndKey.sql"),
                                                               login).SingleOrDefault());
            }
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            { 
                throw new ArgumentNullException("user");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                return Task.FromResult((IList<UserLoginInfo>)connection.Query<UserLoginInfo>(FileHandler.ReadFileContent("UserLogin.FindById.sql"), new { user.Id }).ToList());
            }
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                connection.Execute(FileHandler.ReadFileContent("UserLogin.RemoveLoginByProviderAndKey.sql"),
                    new { userId = user.Id, login.LoginProvider, login.ProviderKey });
            }

            return Task.FromResult(0);
        }
    }
}