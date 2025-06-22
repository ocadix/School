using System.ComponentModel.DataAnnotations;

namespace SchoolFronted.Model
{
    public class Student
    {
        public string? Code { get; set; }

        public string Name { get; set; }

        public string NumDocument { get; set; }

        public string Email { get; set; }
    }

    public class RegisterSubjectViewModelStudent
    {
        public string Name { get; set; }

        public string NumDocument { get; set; }

        public string Email { get; set; }
    }
}
