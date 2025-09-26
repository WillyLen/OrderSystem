using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderSystem.Services
{
    public class OtpDao
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString; // Oracle DB connection string(存在Web.config)

        /// <summary>
        /// Insert Otp到資料庫
        /// </summary>
        public void InsertOtp(string sessionId, byte[] secret, int expiryTime)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"INSERT INTO TR04.OtpSecrets (SessionId, Secret, CreatedAt, ExpiryTime)
                               VALUES (:SessionId, :Secret, :CreatedAt, :ExpiryTime)";
                db.Execute(sql, new
                {
                    SessionId = sessionId,
                    Secret = secret,
                    CreatedAt = DateTime.Now,
                    ExpiryTime = expiryTime
                });
            }
        }

        /// <summary>
        /// 用SessionId取得對應之Otp的Secret
        /// </summary>
        public byte[] GetSecretBySessionId(string sessionId)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT Secret FROM TR04.OtpSecrets WHERE SessionId = :SessionId";
                return db.Query<byte[]>(sql, new { SessionId = sessionId }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 用SessionId刪除對應之Otp(用於登入後清除資料及畫面重整後避免綁到同一SessionId)
        /// </summary>
        public void DeleteOtpBySessionId(string sessionId)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"DELETE FROM TR04.OtpSecrets WHERE SessionId = :SessionId";
                db.Execute(sql, new { SessionId = sessionId });
            }
        }

    }
}