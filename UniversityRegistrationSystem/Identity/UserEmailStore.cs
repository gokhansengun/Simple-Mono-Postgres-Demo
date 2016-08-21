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
    public partial class CustomUserStore<TUser> : IUserEmailStore<TUser> where TUser : CustomUser
    {
        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                { 
                    connection.Execute(FileHandler.ReadFileContent("User.UpdateEmailById.sql"), user);
                }
            });
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                { 
                    connection.Execute(FileHandler.ReadFileContent("User.UpdateEmailConfirmedById.sql"), user);
                }
            });
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                { 
                    return connection.Query<TUser>(FileHandler.ReadFileContent("User.FindByEmail.sql"), new { email }).SingleOrDefault();
                }
            });
        }
    }
}