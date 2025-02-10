using System.ComponentModel.DataAnnotations;

namespace PatientSync.Server.Models
{
    /// <summary>
    /// Represents an application user
    /// </summary>
    public class User
    {
        public int ID { get; set; } // Unique identifier for the user.



        [Required(ErrorMessage = "USERNAME IS REQUIRED!")]
        [StringLength(100, ErrorMessage = "USERNAME CANNOT BE LONGER THAN 100 CHARACTERS!")]
        public string Username { get; set; } // The username used for login.



        [Required(ErrorMessage = "PASSWORD IS REQUIRED!")]
        [DataType(DataType.Password)]
        [StringLength(250, ErrorMessage = "PASSWORD CANNOT BE LONGER THAN 250 CHARACTERS!")]
        public string Password { get; set; } // The user's password. NOTE: In production, store a hashed password instead of plain text!
    }
}
