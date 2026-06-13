using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context,
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.Username))
                    return BadRequest(new AuthResponse { Success = false, Message = "Username is required" });

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(new AuthResponse { Success = false, Message = "Email is required" });

                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                    return BadRequest(new AuthResponse { Success = false, Message = "Password must be at least 6 characters" });

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

                if (existingUser != null)
                    return BadRequest(new AuthResponse { Success = false, Message = "Username or email already exists" });

                // Create new user
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = _authService.HashPassword(request.Password),
                    Role = request.Role,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = _authService.GenerateToken(user);

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new AuthResponse { Success = false, Message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new AuthResponse { Success = false, Message = "Username and password are required" });

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
                    return Unauthorized(new AuthResponse { Success = false, Message = "Invalid username or password" });

                if (!user.IsActive)
                    return Unauthorized(new AuthResponse { Success = false, Message = "User account is inactive" });

                var token = _authService.GenerateToken(user);

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new AuthResponse { Success = false, Message = "An error occurred during login" });
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ChangePasswordResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                    return BadRequest(new ChangePasswordResponse { Success = false, Message = "Current password is required" });

                if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
                    return BadRequest(new ChangePasswordResponse { Success = false, Message = "New password must be at least 6 characters" });

                if (request.NewPassword != request.ConfirmPassword)
                    return BadRequest(new ChangePasswordResponse { Success = false, Message = "New password and confirm password do not match" });

                // Get current user from token (assuming middleware extracts user info)
                var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                    return Unauthorized(new ChangePasswordResponse { Success = false, Message = "User not authenticated" });

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new ChangePasswordResponse { Success = false, Message = "User not found" });

                // Verify current password
                if (!_authService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    return BadRequest(new ChangePasswordResponse { Success = false, Message = "Current password is incorrect" });

                // Update password
                user.PasswordHash = _authService.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new ChangePasswordResponse { Success = true, Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return StatusCode(500, new ChangePasswordResponse { Success = false, Message = "An error occurred while changing password" });
            }
        }
    }
}
