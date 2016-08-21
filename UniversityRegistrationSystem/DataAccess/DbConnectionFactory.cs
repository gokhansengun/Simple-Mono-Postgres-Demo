using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;

namespace UniversityRegistrationSystem.DataAccess
{
    public class DbConnectionFactory
    {
        static string _connectionString;

        static DbConnectionFactory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["NpgsqlConnection"].ConnectionString;
        }

        public static DbConnection NewDbConnection()
        {
            return NewDbConnection(_connectionString);
        }

        public static DbConnection NewDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}