using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoClaseQ4.Services;
using ProyectoClaseQ4.DTOs;
using ProyectoClaseQ4.Models;

namespace ProyectoClaseQ4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly FirebaseServices _firebaseService;
        private readonly IAuthService _authService;

        public UsersController(FirebaseServices firebaseService, IAuthService authService)
        {
            _firebaseService = firebaseService;
            _authService = authService;
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var usersCollection = _firebaseService.GetCollection("grupo_6_usuarios");
                var snapshot = await usersCollection.GetSnapshotAsync();

                var users = new List<object>();

                foreach (var document in snapshot.Documents)
                {
                    var userData = document.ToDictionary();
                    userData.Remove("PasswordHash"); 
                    users.Add(userData);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _authService.GetUserById(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var userDoc = await _firebaseService
                    .GetCollection("grupo_6_usuarios")
                    .Document(id)
                    .GetSnapshotAsync();

                if (!userDoc.Exists)
                {
                    return NotFound(new { error = "Usuario no encontrado" });
                }

                var userData = userDoc.ToDictionary();
                userData.Remove("PasswordHash");

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("role/{role}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var usersCollection = _firebaseService.GetCollection("grupo_6_usuarios");
                var query = usersCollection.WhereEqualTo("Role", role);
                var snapshot = await query.GetSnapshotAsync();

                var users = new List<object>();
                foreach (var document in snapshot.Documents)
                {
                    var userData = document.ToDictionary();
                    userData.Remove("PasswordHash");
                    users.Add(userData);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
