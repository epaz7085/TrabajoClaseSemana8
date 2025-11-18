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
        
        //Register
        public async Task<AuthResponseDto> Register(RegisterDto registerdto)
        {
            try
            {
                //1. Verificar si el usuario ya existe
                var exisingUser = await GetUserByEmail(registerdto.Email);

                if (exisingUser != null)
                {
                    throw new Exception("User with this email already exists");
                }
                
                //2. Generar un ID unico para el usuario
                var userId = Guid.NewGuid().ToString();
                
                //3. Hashear la password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerdto.Password);
                
                //4. Crear documento de usuario en Firestore
                var user = new User
                {
                    Id = userId,
                    Email = registerdto.Email,
                    Fullname = registerdto.FullName,
                    Role = "user",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                };
                
                var usersCollection = _firebaseService.GetCollection("users");
                
                // Guardar usuario con el password hasheado

                var userData = new Dictionary<string, object>()
                {
                    {"Id", user.Id},
                    {"Email", user.Email},
                    {"Fullname", user.Fullname},
                    {"Role", user.Role},
                    {"CreatedAt", user.CreatedAt},
                    {"IsActive", user.IsActive},
                    {"PasswordHash", passwordHash}
                };

                await usersCollection.Document(user.Id).SetAsync(userData);
                
                // 5. Generar token JWT
                var token = GenerateJwtToken(user);
                
                //6. Retornar respuestas
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.Fullname,
                    Role = user.Role
                };

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar usuario: {ex.Message}");
            }
        }
        
        //Login
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            try
            {
                //1. Obtener usuario de Firebase por correo
                var userCollection = _firebaseService.GetCollection("users");
                var query = userCollection.WhereEqualTo("Email", loginDto.Email).Limit(1);
                var snapshot = await query.GetSnapshotAsync();

                if (snapshot.Count == 0)
                {
                    throw new Exception("Credenciales invalidas");
                }
                var userDoc = snapshot.Documents[0];
                
                //2. Extraer campos manualmente
                var userData = userDoc.ToDictionary();
                if (userData.ContainsKey("PasswordHash"))
                {
                    throw new Exception("Usuario sin contrasenia  configurada");
                }
                var passwordHash = userData["PasswordHash"].ToString();
                
                //3. Crear el objeto User
                var user = new User
                {
                    Id = userData.ContainsKey("Id") ? userData["Id"].ToString() : userDoc.Id,
                    Email = userData.ContainsKey("Email").ToString() ?? string.Empty,
                    Fullname = userData.ContainsKey("FullName").ToString() ?? string.Empty,
                    Role = userData.ContainsKey("Role") ? userData["Role"].ToString() ?? "user" : "user",
                    IsActive = userData.ContainsKey("IsActive") && Convert.ToBoolean(userData["IsActive"]),
                    CreatedAt = userData.ContainsKey("CreatedAt")
                        ? ((Timestamp)userData["CreatedAt"]).ToDateTime()
                        : DateTime.UtcNow,
                };
                
                // 4. Verificar contrasenia
                if (string.IsNullOrEmpty(passwordHash) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, passwordHash))
                {
                    throw new Exception("Password invalida");
                }
                //5. Verificar que el usuario este activo
                if (!user.IsActive)
                {
                    throw new Exception("Usuario inactivo");
                }
                
                //6. Generar token JWT
                var token = GenerateJwtToken(user);
                
                //7. Retornar respuesta
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.Fullname,
                    Role = user.Role
                };

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar usuario: {ex.Message}");
            }
        }
    }
}

