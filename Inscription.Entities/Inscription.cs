namespace Inscription.Entities
{
    public class Inscription
    {
        public string? Code { get; set; }

        public required string CodeStudent { get; set; }

        public required string CodeSubjects { get; set; }
    }

    public class InscriptionList
    {
        public required string Code { get; set; }

        public required string CodeStudent { get; set; }

        public required string NameStudent { get; set; }

        public required string NumDocument { get; set; }

        public required string Email { get; set; }

        public required string CodeSubjects { get; set; }

        public required string NameSubjects { get; set; }

        public int Credits { get; set; }
    }
}
