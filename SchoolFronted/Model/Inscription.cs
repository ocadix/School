namespace SchoolFronted.Model
{
    public class Inscription
    {
        public string Code { get; set; }

        public string CodeStudent { get; set; }

        public string NameStudent { get; set; }

        public string NumDocument { get; set; }

        public string Email { get; set; }

        public string CodeSubjects { get; set; }

        public string NameSubjects { get; set; }

        public int Credits { get; set; }
    }

    public class RegisterSubjectViewModelInscription
    {
        public string CodeStudent { get; set; }

        public string CodeSubjects { get; set; }
    }

}
