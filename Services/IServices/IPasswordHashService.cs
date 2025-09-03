namespace PROYEC_QUIMPAC.Services.IServices
{
    public interface IPasswordHashService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        bool IsPasswordHashed(string password);
    }
}