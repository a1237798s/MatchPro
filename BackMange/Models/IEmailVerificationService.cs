namespace BackMange.Models
{
    public interface IEmailVerificationService
    {
        Task<string> GenerateVerificationTokenAsync(int userId);
        Task<bool> VerifyEmailAsync(string token);
        Task<string> GeneratePasswordResetTokenAsync(int userId);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
