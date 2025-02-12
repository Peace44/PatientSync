using Serilog;
using ILogger = Serilog.ILogger;

namespace PatientSync.Server.Utilities
{
    public static class SecurityUtils
    {
        private static readonly ILogger Logger = Log.ForContext(typeof(SecurityUtils));

        // This method is used to generate a hash of a password using a salt
        public static string GeneratePasswordHash(string password, string salt)
        {
            Logger.Information("Generating password hash.");
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
                var hash = System.Convert.ToBase64String(hashedBytes);
                Logger.Information("Password hash generated successfully.");
                return hash;
            }
        }

        // This method is used to generate a random salt
        public static string GenerateSalt()
        {
            Logger.Information("Generating random salt.");
            var bytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            var salt = System.Convert.ToBase64String(bytes);
            Logger.Information("Random salt generated successfully.");
            return salt;
        }

        // This method is used to verify if a given password and salt match the provided hash
        public static bool VerifyPasswordHash(string password, string salt, string hash)
        {
            Logger.Information("Verifying password hash.");
            var generatedHash = GeneratePasswordHash(password, salt);
            var isMatch = generatedHash == hash;
            if (isMatch)
            {
                Logger.Information("Password hash verification succeeded.");
            }
            else
            {
                Logger.Warning("Password hash verification failed.");
            }
            return isMatch;
        }
    }
}
