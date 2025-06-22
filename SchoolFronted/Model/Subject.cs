using System.ComponentModel.DataAnnotations;

namespace SchoolFronted.Model
{
    public class Subject
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int Credits { get; set; }
    }

    public class RegisterSubjectViewModel
    {
        [Required(ErrorMessage = "El nombre de la materia es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Los créditos son obligatorios.")]
        [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10.")]
        public int Credits { get; set; }
    }
}
