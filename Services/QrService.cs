using System;
using QRCoder;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderSystem.Models;

namespace OrderSystem.Services
{
    public class QrService
    {
        public (string, string, LoginSession) GenerateQRCode()
        {
            LoginSession loginSession = new LoginSession
            {
                sessionId = Guid.NewGuid().ToString(),
                expiry = DateTime.Now.AddMinutes(1),
                isUsed = false
            };
            string qrText = $"https://localhost:44337/Auth/LoginAndRegister?sessionId={loginSession.sessionId}";
            byte[] qrCodeImage;
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                qrCodeImage = qrCode.GetGraphic(5);
            }

            string qrCodeImageBase64 = Convert.ToBase64String(qrCodeImage);
            string qrCodeImagedata = "data:image/png;base64," + qrCodeImageBase64;
            return (qrCodeImagedata, qrText, loginSession);
        }
    }
}