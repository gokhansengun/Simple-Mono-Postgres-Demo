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

            user.Id = Guid.NewGuid().ToString();

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                connection.Execute(FileHandler.ReadFileContent("User.Create.sql"), user);
            }

            return Task.FromResult(0);
        }

        public virtual Task DeleteAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                connection.Execute(FileHandler.ReadFileContent("User.DeleteById.sql"), new { user.Id });

                return Task.FromResult(0);
            }
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

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                return Task.FromResult(connection.Query<TUser>(FileHandler.ReadFileContent("User.FindById.sql"), new { id = parsedUserId.ToString() }).SingleOrDefault());
            }
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                var result = connection.Query<TUser>(FileHandler.ReadFileContent("User.FindByName.sql"), new { userName }).SingleOrDefault();

                return Task.FromResult<TUser>(result);
            }
        }

        public virtual Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                connection.Execute(FileHandler.ReadFileContent("User.Update.sql"), user);

                return Task.FromResult(0);
            }
        }
    }
}
