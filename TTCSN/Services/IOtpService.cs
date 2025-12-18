namespace TTCSN.Services
{
    public interface IOtpService
    {
        string GenerateOtp();
        bool ValidateOtp(string userId, string otp);
        void StoreOtp(string accountName, string otp);

    }
}
