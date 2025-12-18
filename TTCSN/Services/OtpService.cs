namespace TTCSN.Services
{
    public class OtpService : IOtpService
    {
        private static Dictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

        public string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // OTP 6 số
        }

        public void StoreOtp(string userId, string otp)
        {
            _otpStore[userId] = (otp, DateTime.UtcNow.AddMinutes(5)); // Hết hạn sau 5 phút
        }

        public bool ValidateOtp(string userId, string otp)
        {
            if (_otpStore.TryGetValue(userId, out var stored))
            {
                if (stored.Expiry > DateTime.UtcNow && stored.Otp == otp)
                {
                    _otpStore.Remove(userId); // Xóa OTP sau khi dùng
                    return true;
                }
            }
            return false;
        }
    }
}
