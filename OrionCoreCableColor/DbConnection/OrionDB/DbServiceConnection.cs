using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.DbConnection.OrionDB
{
    public class DbServiceConnection
    {
        public string ConnectionString { get; set; }
        public ORIONDBEntities OrionContext;
        public DbServiceConnection(string connection)
        {
            ConnectionString = connection;
            OrionContext = new ORIONDBEntities();
            OrionContext.Database.Connection.ConnectionString = ConnectionString;

        }
        public List<T> LoadListDataWithDbContext<T>(string Command)
        {
            try
            {
                var list = OrionContext.Database.SqlQuery<T>(Command).ToList();
                return list;

            }
            catch (Exception e)
            {
                return default;
            }

        }
        public bool PostCommandWithDbContext(string Command)
        {
            try
            {
                var result = OrionContext.Database.ExecuteSqlCommand(Command) > 0;
                return result;

            }
            catch (Exception e)
            {
                return default;
            }

        }
        public bool PostCommandWithDbContext(string Command, params SqlParameter[] parameters)
        {
            try
            {
                var command = GenerateCommandText(Command, parameters);
                var result = OrionContext.Database.ExecuteSqlCommand(GenerateCommandText(Command, parameters), parameters) > 0;

                return result;

            }
            catch (Exception e)
            {
                return default;
            }

        }








        //HELPERS

        public static string GenerateCommandText(string storedProcedure, SqlParameter[] parameters)
        {
            string CommandText = "EXEC {0} {1}";
            string[] ParameterNames = new string[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterNames[i] = parameters[i].ParameterName;
            }

            return string.Format(CommandText, storedProcedure, string.Join(",", ParameterNames));
        }
        public class ResponseResultDataFromSP
        {
            public bool Success { get; set; }
            public string Message { get; set; }

            public ResponseResultDataFromSP() { }

            public ResponseResultDataFromSP(bool result, string message)
            {
                Success = result;
                Message = message;
            }

        }

    }
}