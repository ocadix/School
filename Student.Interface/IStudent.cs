namespace Student.Interface
{
    public interface IStudent
    {
        Task<IList<Entities.Student>> ListStudent();

        Task<IList<Entities.Student>> ListStudentDocument(string Code);

        Task<bool> DeleteStudent(string code);

        Task<bool> SaveOrUpdateStudent(Entities.Student student);
    }
}
