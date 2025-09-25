using Dapper;
using Oracle.ManagedDataAccess.Client;
using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace OrderSystem.Services
{
    public class LoginSessionDao
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString;

        public void Insert(LoginSession session)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"INSERT INTO TR04.LoginSessions (SessionId, Account, Expiry, IsUsed, CreatedAt)
                               VALUES (:SessionId, :Account, :Expiry, :IsUsed, :CreatedAt)";
                db.Execute(sql, new
                {
                    SessionId = session.sessionId,
                    Account = session.account,
                    Expiry = session.expiry,
                    IsUsed = session.isUsed ? 1 : 0,
                    CreatedAt = session.createdAt
                });
            }
        }

        public LoginSession GetBySessionId(string sessionId)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT * FROM TR04.LoginSessions WHERE SessionId = :SessionId";
                return db.Query<LoginSession>(sql, new { SessionId = sessionId }).FirstOrDefault();
            }
        }

        public void MarkAsUsedBySessionId(string sessionId)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"UPDATE TR04.LoginSessions SET IsUsed = 1 WHERE SessionId = :SessionId";
                db.Execute(sql, new { SessionId = sessionId });
            }
        }

        public void MarkAccount(string sessionId, string account)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"UPDATE TR04.LoginSessions SET Account = :account WHERE SessionId = :SessionId";
                db.Execute(sql, new { SessionId = sessionId, Account = account });
            }
        }
    }
}
