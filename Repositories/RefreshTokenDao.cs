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
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString; // Oracle DB connection string(存在Web.config)

        /// <summary>
        /// 用Token去找出對應的帳號
        /// </summary>
        public string GetAccountByToken(string token)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT Account FROM TR04.RefreshTokens 
                       WHERE Token = :Token AND Expiry > SYSDATE";
                return db.Query<string>(sql, new { Token = token }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 儲存Refresh Token到資料庫
        /// </summary>
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

        /// <summary>
        /// 用Account取得對應的Refresh Token(且未過期)
        /// </summary>
        public string GetTokenByAccount(string account)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT Token FROM TR04.RefreshTokens WHERE Account = :Account AND Expiry > SYSDATE";
                return db.Query<string>(sql, new { Account = account }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 用於登出時刪除對應帳號的Refresh Token
        /// </summary>
        /// <param name="account"></param>
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