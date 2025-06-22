using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Student.Entities;
using Student.Interface;
using System.Data;

namespace Student.Impl
{
    public class StudentService(IConfiguration configuration) : IStudent
    {
        private readonly IConfigurationSection configurationSection = configuration.GetSection("AppIdentitySettings").GetSection("Student");
        private readonly IConfigurationSection configurationConnection = configuration.GetSection("AppIdentitySettings").GetSection("Connection").GetSection("ConnectionString");


        public async Task<bool> DeleteStudent(string code)
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

        public async Task<IList<Entities.Student>> ListStudent()
        {
            var subjectsList = new List<Entities.Student>();

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
                var subject = new Entities.Student
                {
                    Code = reader["Code"].ToString(),
                    Name = reader["Name"]?.ToString() ?? "",
                    NumDocument = reader["NumDocument"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                };

                subjectsList.Add(subject);
            }

            return subjectsList;
        }

        public async Task<IList<Entities.Student>> ListStudentDocument(string Code)
        {
            var subjectsList = new List<Entities.Student>();

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("ListDocument").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Code", SqlDbType.NVarChar).Value = Code;

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var subject = new Entities.Student
                {
                    Code = reader["Code"].ToString(),
                    Name = reader["Name"]?.ToString() ?? "",
                    NumDocument = reader["NumDocument"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                };

                subjectsList.Add(subject);
            }

            return subjectsList;
        }

        public async Task<bool> SaveOrUpdateStudent(Entities.Student student)
        {
            bool isSaveOrUpdate = false;

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("SaveOrUpdate").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (!string.IsNullOrWhiteSpace(student.Code) && long.TryParse(student.Code, out var parsedCode))
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = parsedCode;
            }
            else
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = DBNull.Value;
            }

            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = student.Name;
            cmd.Parameters.Add("@NumDocument", SqlDbType.NVarChar).Value = student.NumDocument;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = student.Email;

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
