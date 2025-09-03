using PROYEC_QUIMPAC.Services.IServices;
using BCrypt.Net;

namespace PROYEC_QUIMPAC.Services
{
    public class PasswordHashService : IPasswordHashService
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public bool IsPasswordHashed(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // BCrypt hashes always start with $2a$, $2b$, $2x$, or $2y$ and have a specific length
            return password.StartsWith("$2") && password.Length >= 59;
        }
    }
}