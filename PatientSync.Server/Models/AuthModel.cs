using System.ComponentModel.DataAnnotations;

namespace PatientSync.Server.Models
{
    /// <summary>
    /// Represents authentication credentials submitted by a user for registration or login.
    /// </summary>
    public class AuthModel
    {
        [Required(ErrorMessage = "USERNAME IS REQUIRED!")]
        public string Username { get; set; }


        [Required(ErrorMessage = "PASSWORD IS REQUIRED!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
