using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using ProyectoClaseQ4.DTOs;
using ProyectoClaseQ4.Models;

namespace ProyectoClaseQ4.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly IConfiguration _configuration;

        public AuthService(FirebaseServices firebaseService, IConfiguration configuration)
        {
            _firebaseService = firebaseService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Register(RegisterDto dto)
        {
            try
            {
                var existing = await GetUserByEmail(dto.Correo);
                if (existing != null)
                    throw new Exception("El correo ya está registrado.");

                var userId = Guid.NewGuid().ToString();
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var user = new User
                {
                    Id = userId,
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Correo = dto.Correo,
                    PasswordHash = passwordHash,
                    Edad = dto.Edad,
                    NumeroIdentidad = dto.NumeroIdentidad,
                    Genero = dto.Genero,
                    NumeroTelefono = dto.NumeroTelefono,
                    EsDonadorOrganos = dto.EsDonadorOrganos,
                    Rol = "user",
                    FechaRegistro = Timestamp.GetCurrentTimestamp()
                };

                var col = _firebaseService.GetCollection("grupo_6_usuarios");
                await col.Document(userId).SetAsync(user);

                var token = GenerateJwtToken(user);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Correo = user.Correo,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Rol = user.Rol
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar usuario: {ex.Message}");
            }
        }

        public async Task<User?> GetUserByEmail(string correo)
        {
            try
            {
                var col = _firebaseService.GetCollection("grupo_6_usuarios");
                var query = col.WhereEqualTo("Correo", correo).Limit(1);
                var snap = await query.GetSnapshotAsync();

                if (snap.Count == 0)
                    return null;

                return snap.Documents[0].ConvertTo<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            try
            {
                var user = await GetUserByEmail(dto.Correo);
                if (user == null)
                    throw new Exception("Credenciales inválidas.");

                if (string.IsNullOrEmpty(user.PasswordHash) ||
                    !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    throw new Exception("Credenciales inválidas.");
                }

                var token = GenerateJwtToken(user);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Correo = user.Correo,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Rol = user.Rol
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al iniciar sesión: {ex.Message}");
            }
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("correo", user.Correo),
                new Claim("nombre", user.Nombre),
                new Claim("apellido", user.Apellido),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User?> GetUserById(string id)
        {
            var doc = await _firebaseService
                .GetCollection("grupo_6_usuarios")
                .Document(id)
                .GetSnapshotAsync();

            return doc.Exists ? doc.ConvertTo<User>() : null;
        }
    }
}
