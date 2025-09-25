using Dapper;
using Oracle.ManagedDataAccess.Client;
using OrderSystem.Models;
using OtpNet;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace OrderSystem.Services
{
    public class OtpService
    {

        private readonly OtpDao otpDao = new OtpDao();

        public OtpEnrollment GenerateOtp(string sessionId)
        {
            // 產生 OTP Secret
            var secret = KeyGeneration.GenerateRandomKey(20);
            var totp = new Totp(secret, step: 60);
            var otpCode = totp.ComputeTotp();

            // 儲存到資料庫
            otpDao.InsertOtp(sessionId, secret, 60);

            // 回傳 OTP 模型給前端
            return new OtpEnrollment
            {
                sessionId = sessionId,
                otpCode = otpCode,
                expiryTime = 60
            };
        }

        public bool ValidateOtp(string sessionId, string inputOtp)
        {
            // 從資料庫取得 Secret
            var secret = otpDao.GetSecretBySessionId(sessionId);
            if (secret == null)
                return false;

            var totp = new Totp(secret, step: 60);
            return totp.VerifyTotp(inputOtp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
        public void DeleteOtp(string sessionId)
        {
            otpDao.DeleteOtp(sessionId);
        }

    }
}
