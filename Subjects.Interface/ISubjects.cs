namespace Subjects.Interface
{
    public interface ISubjects
    {
        Task<IList<Entities.Subjects>> ListSubjects();

        Task<Entities.Subjects> ListSubjectsCredits(string Code);

        Task<bool> DeleteSubjects(string code);

        Task<bool> SaveOrUpdateSubjects(Entities.Subjects subjects);

    }
}
