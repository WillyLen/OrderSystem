using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderSystem.Repositories
{
    public class RefreshTokenDao
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString;

        public string GetAccountByToken(string token)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT Account FROM TR04.RefreshTokens 
                       WHERE Token = :Token AND Expiry > SYSDATE";
                return db.Query<string>(sql, new { Token = token }).FirstOrDefault();
            }
        }


        public void SaveToken(string account, string token, DateTime expiry)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"INSERT INTO TR04.RefreshTokens (TokenId, Account, Token, Expiry, CreatedAt)
                           VALUES (:TokenId, :Account, :Token, :Expiry, :CreatedAt)";
                db.Execute(sql, new
                {
                    TokenId = Guid.NewGuid().ToString(),
                    Account = account,
                    Token = token,
                    Expiry = expiry,
                    CreatedAt = DateTime.Now
                });
            }
        }

        public string GetToken(string account)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT Token FROM TR04.RefreshTokens WHERE Account = :Account AND Expiry > SYSDATE";
                return db.Query<string>(sql, new { Account = account }).FirstOrDefault();
            }
        }

        public void DeleteToken(string account)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"DELETE FROM TR04.RefreshTokens WHERE Account = :Account";
                db.Execute(sql, new { Account = account });
            }
        }
    }

}