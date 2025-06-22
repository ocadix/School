using Inscription.Entities;
using Inscription.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Student.Interface;
using Subjects.Interface;
using System.Data;

namespace Inscription.Impl
{
    public class InscriptionService(IConfiguration configuration, IStudent student, ISubjects subjects) : IInscription
    {

        private readonly IConfigurationSection configurationSection = configuration.GetSection("AppIdentitySettings").GetSection("Inscription");
        private readonly IConfigurationSection configurationConnection = configuration.GetSection("AppIdentitySettings").GetSection("Connection").GetSection("ConnectionString");
        private readonly ISubjects _subjects = subjects;
        private readonly IStudent _student = student;

        public async Task<bool> DeleteInscription(string code)
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

        public async Task<IList<InscriptionList>> ListInscription(string? code)
        {
            var inscriptionList = new List<InscriptionList>();

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("List").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

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
                var subject = new InscriptionList
                {
                    Code = reader["Code"].ToString() ?? "",
                    CodeStudent = reader["CodeStudent"].ToString() ?? "",
                    NameStudent = reader["NameStudent"].ToString() ?? "",
                    NumDocument = reader["NumDocument"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    CodeSubjects = reader["CodeSubjects"].ToString() ?? "",
                    NameSubjects = reader["NameSubjects"].ToString() ?? "",
                    Credits = reader["Credits"] is DBNull ? 0 : Convert.ToInt32(reader["Credits"])
                };

                inscriptionList.Add(subject);
            }

            return inscriptionList;
        }

        public async Task<bool> SaveOrUpdateInscription(Entities.Inscription inscription)
        {
            bool isSaveOrUpdate = false;

            var connectionString = configurationConnection.Value ?? throw new InvalidOperationException("Connection string is missing.");
            var storedProcName = configurationSection.GetSection("SaveOrUpdate").Value ?? throw new InvalidOperationException("Stored procedure name is missing.");

            var studentList = await ListInscription(inscription.CodeStudent);
            if (studentList.Count == 0)
            {
                ///Si entra al condicional quiere decir que no tiene materias registradas...
                ///throw new InvalidOperationException("Materias no registradas");

                var studen = await _student.ListStudentDocument(inscription.CodeStudent);
                if (studen.Count == 0)
                    throw new InvalidOperationException("Estudiante no encontrado");
            }

            var subjectsList = await _subjects.ListSubjectsCredits(inscription.CodeSubjects);
            if (subjectsList.Code == null)
            {
                throw new InvalidOperationException("Materia no encontrada");
            }

            /// Contar cuántas materias con más de 4 créditos tiene actualmente el estudiante
            int materiasAltosCreditos = studentList.Count(em => em.Credits >= 4);

            /// Si la materia a inscribir también es mayor a 4 créditos
            if (subjectsList.Credits >= 4 && materiasAltosCreditos >= 3)
            {
                throw new InvalidOperationException("El estudiante ya tiene 3 materias con más de 4 créditos.");
            }

            /// Verificar si ya está inscrito
            if (studentList.Any(em => em.CodeSubjects == inscription.CodeSubjects))
            {
                throw new InvalidOperationException("El estudiante ya está inscrito en esta materia.");
            }

            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(storedProcName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Agregar el parámetro @Code, permitiendo null si 'code' no es válido
            if (!string.IsNullOrWhiteSpace(inscription.Code) && long.TryParse(inscription.Code, out var parsedCode))
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = parsedCode;
            }
            else
            {
                cmd.Parameters.Add("@Code", SqlDbType.BigInt).Value = DBNull.Value;
            }

            cmd.Parameters.Add("@CodeStudent", SqlDbType.NVarChar).Value = inscription.CodeStudent;
            cmd.Parameters.Add("@CodeSubjects", SqlDbType.NVarChar).Value = inscription.CodeSubjects;

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
