using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using System.Configuration;
using UniversityRegistrationSystem.DataAccess;
using UniversityRegistrationSystem.Utility;

namespace UniversityRegistrationSystem.Identity
{
    public class CustomRoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
            where TRole : CustomRole
    {
        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            throw new NotImplementedException();
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            throw new NotImplementedException();
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            using (var connection = DbConnectionFactory.NewDbConnection())
            { 
                return Task.FromResult<TRole>(connection.Query<TRole>(FileHandler.ReadFileContent("UserRole.FindById.sql"), roleId).SingleOrDefault());
            }
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException("roleName");
            }

            using (var connection = DbConnectionFactory.NewDbConnection())
            { 
                return Task.FromResult<TRole>(connection.Query<TRole>(FileHandler.ReadFileContent("UserRole.FindByName.sql"), new { Name = roleName }).SingleOrDefault());
            }
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            throw new NotImplementedException();
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                using (var connection = DbConnectionFactory.NewDbConnection())
                {
                    var result = connection.Query<TRole>(FileHandler.ReadFileContent("Roles.GetAll.sql"));

                    return result.AsQueryable();
                }
            }
        }

        public Task<TRole> FindByIdAsync(Guid roleId)
        {
            return FindByIdAsync(roleId.ToString());
        }

        public void Dispose()
        {
        }
    }
}