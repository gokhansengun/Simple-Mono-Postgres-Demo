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
    public partial class CustomUserStore<TUser> : IUserStore<TUser> where TUser : CustomUser
    {        
        public virtual Task CreateAsync(TUser user)
        {
            if (user == null)
            { 
                throw new ArgumentNullException("user");
            }

            return Task.Factory.StartNew(() => {
                user.Id = Guid.NewGuid().ToString();

                using (var connection = DbConnectionFactory.NewDbConnection())
                { 
                    connection.Execute(FileHandler.ReadFileContent("User.Create.sql"), user);
                }
            });
        }

        public virtual Task DeleteAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                {
                    connection.Execute(FileHandler.ReadFileContent("User.DeleteById.sql"), new { user.Id });
                }
            });
        }

        public virtual Task<TUser> FindByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            { 
                throw new ArgumentNullException("id");
            }

            Guid parsedUserId;
            if (!Guid.TryParse(id, out parsedUserId))
            { 
                throw new ArgumentOutOfRangeException("id", string.Format("'{0}' is not a valid GUID.", new { id }));
            }

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                { 
                    return connection.Query<TUser>(FileHandler.ReadFileContent("User.FindById.sql"), new { id = parsedUserId.ToString() }).SingleOrDefault();
                }
            });
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName");
            }

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                {
                    return connection.Query<TUser>(FileHandler.ReadFileContent("User.FindByName.sql"), new { userName }).SingleOrDefault();
                }
            });
        }

        public virtual Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                {
                    connection.Execute(FileHandler.ReadFileContent("User.Update.sql"), user);
                }
            });
        }
    }
}
