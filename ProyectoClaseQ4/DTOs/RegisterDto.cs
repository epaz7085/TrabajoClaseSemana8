using System.ComponentModel.DataAnnotations;

namespace ProyectoClaseQ4.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Range(1, 120)]
        public int Edad { get; set; }

        [Required]
        public string NumeroIdentidad { get; set; }

        [Required]
        public string Genero { get; set; }

        [Required]
        public string NumeroTelefono { get; set; }

        [Required]
        public bool EsDonadorOrganos { get; set; }
    }
}
