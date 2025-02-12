using System.ComponentModel.DataAnnotations;

namespace PatientSync.Server.Models
{
    /// <summary>
    /// Represents an application user
    /// </summary>
    public class User
    {
        public int? ID { get; set; } // Unique identifier for the user.



        [Required(ErrorMessage = "USERNAME IS REQUIRED!")]
        [StringLength(128, ErrorMessage = "USERNAME CANNOT BE LONGER THAN 128 CHARACTERS!")]
        public string Username { get; set; } // The username used for login.



        [Required(ErrorMessage = "PASSWORD SALT IS REQUIRED!")]
        [StringLength(256, ErrorMessage = "PASSWORD SALT CANNOT BE LONGER THAN 256 CHARACTERS!")]
        public string PasswordSalt { get; set; }
        


        [Required(ErrorMessage = "PASSWORD HASH IS REQUIRED!")]
        [DataType(DataType.Password)]
        [StringLength(256, ErrorMessage = "PASSWORD HASH CANNOT BE LONGER THAN 256 CHARACTERS!")]
        public string PasswordHash { get; set; } // The user's password. NOTE: In production, store a hashed password instead of plain text!



        // Constructor that takes an AuthModel as input
        public User(AuthModel authModel)
        {
            string username = authModel.Username;
            string password = authModel.Password;

            string passwordSalt = Utilities.SecurityUtils.GenerateSalt();
            string passwordHash = Utilities.SecurityUtils.GeneratePasswordHash(password, passwordSalt);

            this.Username = username;
            this.PasswordSalt = passwordSalt;
            this.PasswordHash = passwordHash;
        }
    }
}
