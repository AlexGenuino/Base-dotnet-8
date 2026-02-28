using Domain.Enum;

namespace Domain.Abstractions
{
    public interface IAuthService
    {
        string GenerateJWT(string email, string username);
        string GenerateRefreshToken();
        string HashingPassword(string password);
        bool VerifyPassword(string password, string hash);
        ValidationFieldsUser? GetValidationErrorForEmailAndUsername(bool emailExists, bool usernameExists);
    }
}
