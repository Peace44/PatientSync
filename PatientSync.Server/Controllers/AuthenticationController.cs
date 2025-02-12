using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using PatientSync.Server.Models;
using PatientSync.Server.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PatientSync.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        // Constructor injection of the User Service
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        // POST /authentication/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthModel authModel)
        {
            string username = authModel.Username;
            string password = authModel.Password;

            if (!ModelState.IsValid)
            {
                Log.Warning("INVALID LOGIN MODEL RECEIVED: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            Log.Information("Login attempt for username: {Username}", username);

            // Validate the user's credentials
            var user = _userService.GetUserByUsername(username);
            if (user == null)
            {
                Log.Warning("USER WITH USERNAME '{Username}' NOT FOUND!", username);
                return Unauthorized("INVALID USERNAME OR PASSWORD!");
            }

            string passwordHash = user.PasswordHash;
            string passwordSalt = user.PasswordSalt;
            if (!Utilities.SecurityUtils.VerifyPasswordHash(password, passwordSalt, passwordHash))
            {
                Log.Warning("PASSWORD MISMATCH FOR USER '{Username}'!", username);
                return Unauthorized("INVALID USERNAME OR PASSWORD!");
            }

            // Create a list of claims based on the user's information
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
                // Additional claims can be added here if needed (e.g., roles)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Optionally, set authentication properties (e.g., persistent cookies)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // "Remember me" functionality
                // You can add ExpiresUtc, IssuedUtc, etc...
            };

            // Sign in the user by issuing the authentication cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            Log.Information("User '{Username}' logged in successfully!", username);

            return Ok($"Login successful for username '{username}'!");
        }

        // POST /authentication/register (New User Registration)
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthModel authModel)
        {
            string username = authModel.Username;

            if (!ModelState.IsValid)
            {
                Log.Warning("INVALID REGISTRATION MODEL RECEIVED: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            Log.Information("User registration attempt for username: {Username}", username);

            // Check if the username is already taken
            if (_userService.GetUserByUsername(username) != null)
            {
                Log.Warning("USERNAME '{Username}' ALREADY REGISTERED!", username);
                return BadRequest($"USERNAME '{username}' ALREADY REGISTERED!");
            }

            var newUser = new User(authModel);

            _userService.AddUser(newUser);

            Log.Information("User '{Username}' registered successfully with ID = '{ID}'", username, _userService.GetUserByUsername(username).ID);

            return Created($"/user/{newUser.ID}", new { id = newUser.ID, username = username });
        }

        // POST /authentication/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Log.Information("User logged out successfully.");
            return Ok("Logout successful!");
        }

        // GET /authentication/me
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Log.Warning("UNAUTHORIZED ACCESS ATTEMPT TO /authentication/me");
                return Unauthorized("USER NOT AUTHENTICATED!");
            }

            var username = User.FindFirstValue(ClaimTypes.Name);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Log.Information("User '{Username}' retrieved their profile.", username);

            return Ok(new
            {
                ID = userId,
                Username = username
            });
        }
    }
}
