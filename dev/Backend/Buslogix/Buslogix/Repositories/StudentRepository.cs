using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class StudentRepository(IDataAccess dataAccess) : IStudentRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<Student?> GetStudent(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Student> rows = await _dataAccess.ExecuteReader("get_student", CommandType.StoredProcedure,
                static reader => new Student
                {
                    Id = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1),
                    LastName = reader.GetStringOrDefault(2),
                    Address = reader.GetStringOrDefault(3),
                    IdentityDocument = reader.GetStringOrDefault(4),
                    RouteId = reader.GetInt32OrDefault(5),
                    GradeId = reader.GetInt32OrDefault(6),
                    IsActive = reader.GetBooleanOrDefault(7)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Student>> GetStudents(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null,
            int? routeId = null,
            int? gradeId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_is_active"] = isActive,
                ["p_identity_document"] = identityDocument,
                ["p_name"] = name,
                ["p_lastname"] = lastName,
                ["p_route_id"] = routeId,
                ["p_grade_id"] = gradeId
            };

            List<Student> rows = await _dataAccess.ExecuteReader("get_students", CommandType.StoredProcedure,
                static reader => new Student
                {
                    Id = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1),
                    LastName = reader.GetStringOrDefault(2),
                    IdentityDocument = reader.GetStringOrDefault(3),
                    RouteId = reader.GetInt32OrDefault(4),
                    GradeId = reader.GetInt32OrDefault(5),
                    IsActive = reader.GetBooleanOrDefault(6)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertStudent(int companyId, Student student)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_name"] = student.Name,
                ["p_lastname"] = student.LastName,
                ["p_address"] = student.Address,
                ["p_identity_document"] = student.IdentityDocument,
                ["p_route_id"] = student.RouteId,
                ["p_grade_id"] = student.GradeId,
                ["p_is_active"] = student.IsActive
            };

            object? result = await _dataAccess.ExecuteScalar("insert_student", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateStudent(int companyId, int id, Student student)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_name"] = student.Name,
                ["p_lastname"] = student.LastName,
                ["p_address"] = student.Address,
                ["p_identity_document"] = student.IdentityDocument,
                ["p_route_id"] = student.RouteId,
                ["p_grade_id"] = student.GradeId,
                ["p_is_active"] = student.IsActive
            };

            return await _dataAccess.ExecuteNonQuery("update_student", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteStudent(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_student", CommandType.StoredProcedure, parameters);
        }
    }
}