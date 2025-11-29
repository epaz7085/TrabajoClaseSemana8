using Google.Cloud.Firestore;

namespace ProyectoClaseQ4.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Nombre { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Apellido { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Correo { get; set; } = string.Empty;

        [FirestoreProperty]
        public string PasswordHash { get; set; } = string.Empty;

        [FirestoreProperty]
        public int Edad { get; set; }

        [FirestoreProperty]
        public string NumeroIdentidad { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Genero { get; set; } = string.Empty;

        [FirestoreProperty]
        public string NumeroTelefono { get; set; } = string.Empty;

        [FirestoreProperty]
        public bool EsDonadorOrganos { get; set; }

        [FirestoreProperty]
        public string Rol { get; set; } = "user";

        [FirestoreProperty]
        public Timestamp FechaRegistro { get; set; } = Timestamp.GetCurrentTimestamp();
    }
}
