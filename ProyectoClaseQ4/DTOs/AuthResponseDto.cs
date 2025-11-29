/*namespace ProyectoClaseQ4.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
    }
}*/

namespace ProyectoClaseQ4.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rol { get; set; }
    }
}
