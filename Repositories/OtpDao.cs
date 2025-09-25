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
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString;

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

        public byte[] GetSecretBySessionId(string sessionId)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT Secret FROM TR04.OtpSecrets WHERE SessionId = :SessionId";
                return db.Query<byte[]>(sql, new { SessionId = sessionId }).FirstOrDefault();
            }
        }

        public void DeleteOtp(string sessionId)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"DELETE FROM TR04.OtpSecrets WHERE SessionId = :SessionId";
                db.Execute(sql, new { SessionId = sessionId });
            }
        }

    }
}