using Dapper;
using Oracle.ManagedDataAccess.Client;
using OrderSystem.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderSystem.Services
{
    /// <summary>
    /// 產生連結至登入頁的QR Code
    /// </summary>
    public class QrService
    {
        LoginSessionDao dao = new LoginSessionDao();
        public (string, string, LoginSession) GenerateQRCode()
        {
            // 建立 LoginSession 資料
            LoginSession loginSession = new LoginSession
            {
                sessionId = Guid.NewGuid().ToString(),
                account = null,
                expiry = DateTime.Now.AddMinutes(1),
                isUsed = false,
                createdAt = DateTime.Now
            };


            // 儲存到資料庫
            dao.Insert(loginSession);

            // 產生 QR Code
            string qrText = $"https://localhost:44337/Auth/Login?sessionId={loginSession.sessionId}";
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