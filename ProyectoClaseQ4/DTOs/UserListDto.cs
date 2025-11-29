namespace ProyectoClaseQ4.DTOs
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public int Edad { get; set; }
        public string Genero { get; set; }
        public string NumeroTelefono { get; set; }
        public bool EsDonadorOrganos { get; set; }
        public string Rol { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
