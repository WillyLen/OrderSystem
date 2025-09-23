using OrderSystem.Models;
using OtpNet;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace OrderSystem.Services
{
    public class OtpService
    {
        // 模擬儲存 OTP 的記憶體資料庫（可改用 Redis 或 DB）
        private static ConcurrentDictionary<string, byte[]> otpSecrets = new ConcurrentDictionary<string, byte[]>();
        public OtpEnrollment GenerateOtp(string sessionId)
        {
            var secret = KeyGeneration.GenerateRandomKey(20);
            otpSecrets[sessionId] = secret;

            var totp = new Totp(secret, step: 60);
            var otpCode = totp.ComputeTotp();

            return new OtpEnrollment
            {
                sessionId = sessionId,
                otpCode = otpCode,
                expiryTime = 60
            };
        }

        public bool ValidateOtp(string sessionId, string inputOtp)
        {
            if (!otpSecrets.TryGetValue(sessionId, out var secret))
                return false;

            var totp = new Totp(secret, step: 60);
            return totp.VerifyTotp(inputOtp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
    }
}
