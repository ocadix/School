using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Subjects.Interface;
using System.Data;

namespace Subjects.Impl
{
    public class SubjectsService(IConfiguration configuration) : ISubjects
    {
        private readonly IConfigurationSection configurationSection = configuration.GetSection("AppIdentitySettings").GetSection("Subjects");
        private readonly IConfigurationSection configurationConnection = configuration.GetSection("AppIdentitySettings").GetSection("Connection").GetSection("ConnectionString");

        public async Task<bool> DeleteSubjects(string code)
        {
            bool isDelete = false;

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("Delete").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Agregar el parámetro @Code, permitiendo null si 'code' no es válido
            if (!string.IsNullOrWhiteSpace(code) && long.TryParse(code, out var parsedCode))
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = parsedCode;
            }
            else
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = DBNull.Value;
            }

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                isDelete = reader[0].ToString() == "1";
            }

            return isDelete;
        }

        public async Task<IList<Entities.Subjects>> ListSubjects()
        {
            var subjectsList = new List<Entities.Subjects>();

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("List").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var subject = new Entities.Subjects
                {
                    Code = reader["Code"].ToString(),
                    Name = reader["Name"]?.ToString() ?? "",
                    Credits = reader["Credits"] is DBNull ? 0 : Convert.ToInt32(reader["Credits"])
                };

                subjectsList.Add(subject);
            }

            return subjectsList;
        }

        public async Task<Entities.Subjects> ListSubjectsCredits(string Code)
        {
            var subjectsList = new Entities.Subjects() { Name = "" };

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("ListCredit").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Agregar el parámetro @Code, permitiendo null si 'code' no es válido
            if (!string.IsNullOrWhiteSpace(Code) && long.TryParse(Code, out var parsedCode))
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = parsedCode;
            }

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var subject = new Entities.Subjects
                {
                    Code = reader["Code"].ToString(),
                    Name = reader["Name"]?.ToString() ?? "",
                    Credits = reader["Credits"] is DBNull ? 0 : Convert.ToInt32(reader["Credits"])
                };

                subjectsList = subject;
            }

            return subjectsList;
        }

        public async Task<bool> SaveOrUpdateSubjects(Entities.Subjects subjects)
        {
            bool isSaveOrUpdate = false;

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("SaveOrUpdate").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Agregar el parámetro @Code, permitiendo null si 'code' no es válido
            if (!string.IsNullOrWhiteSpace(subjects.Code) && long.TryParse(subjects.Code, out var parsedCode))
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = parsedCode;
            }
            else
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = DBNull.Value;
            }

            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = subjects.Name;
            cmd.Parameters.Add("@Credits", SqlDbType.Int).Value = subjects.Credits;

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                isSaveOrUpdate = reader[0].ToString() == "1";
            }

            return isSaveOrUpdate;
        }
    }
}
