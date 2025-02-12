using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PatientSync.Server.Models;
using PatientSync.Server.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PatientSync.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public UserController(IUserService userService, ILogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/user
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            _logger.Information("Getting all users");
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            _logger.Information("Getting user by ID: {UserId}", id);
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                _logger.Warning("User with ID: {UserId} not found", id);
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/user/byusername/{username}
        [HttpGet("byusername/{username}")]
        public ActionResult<User> GetUserByUsername(string username)
        {
            _logger.Information("Getting user by username: {Username}", username);
            var user = _userService.GetUserByUsername(username);
            if (user == null)
            {
                _logger.Warning("User with username: {Username} not found", username);
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public ActionResult AddUser([FromBody] User user)
        {
            _logger.Information("Adding new user with username: {Username}", user.Username);
            _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.ID }, user);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.ID)
            {
                _logger.Warning("User ID mismatch: {UserId} != {UserBodyId}", id, user.ID);
                return BadRequest("User ID mismatch");
            }
            var existing = _userService.GetUserById(id);
            if (existing == null)
            {
                _logger.Warning("User with ID: {UserId} not found", id);
                return NotFound();
            }
            _logger.Information("Updating user with ID: {UserId}", id);
            _userService.UpdateUser(user);
            return NoContent();
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            _logger.Information("Deleting user with ID: {UserId}", id);
            var existing = _userService.GetUserById(id);
            if (existing == null)
            {
                _logger.Warning("User with ID: {UserId} not found", id);
                return NotFound();
            }
            _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
