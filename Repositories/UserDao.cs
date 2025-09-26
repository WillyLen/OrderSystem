using Dapper;
using Oracle.ManagedDataAccess.Client;
using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderSystem.Services
{
    public class UserDao
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString; // Oracle DB connection string(存在Web.config)

        /// <summary>
        /// 用Account取得使用者資料
        /// </summary>
        public User GetUserByAccount(string account)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT account, password, email, role, createdAt, TotpSecret, TwoFactorEnabled
                               FROM TR04.Users 
                               WHERE account = :account";
                var user = db.Query<User>(sql, new { account = account }).FirstOrDefault();
                return user;
            }
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        public void AddUser(User user)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"INSERT INTO TR04.Users (account, password, email, role, createdAt)
                               VALUES (:account, :password, :email, :role, :createdAt)";
                db.Execute(sql, new
                {
                    account = user.account,
                    password = user.password,
                    email = user.email,
                    role = user.role,
                    createdAt = user.createdAt
                });
            }
        }

        /// <summary>
        /// 檢查帳號是否重複
        /// </summary>
        public bool VerifyDuplicateAccount(string account)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT COUNT(*) 
                       FROM TR04.Users 
                       WHERE account = :account";
                int count = db.ExecuteScalar<int>(sql, new { account = account });
                return count > 0; // 如果 count 大於 0，表示帳號已存在
            }
        }

        /// <summary>
        /// 檢查Email是否重複
        /// </summary>
        public bool VerifyDuplicateEmail(string email)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"SELECT COUNT(*) 
                       FROM TR04.Users 
                       WHERE email = :email";
                int count = db.ExecuteScalar<int>(sql, new { email = email });
                return count > 0; // 如果 count 大於 0，表示 Email 已存在
            }
        }

        /// <summary>
        /// 將TOTP密鑰與啟用狀態更新到使用者資料
        /// </summary>
        public void UpdateTotp(string account, string base32Secret, bool enabled)
        {
            using (IDbConnection db = new OracleConnection(connectionString))
            {
                string sql = @"UPDATE TR04.Users 
                       SET TotpSecret = :secret, TwoFactorEnabled = :enabled
                       WHERE account = :account";
                db.Execute(sql, new { secret = base32Secret, enabled = enabled ? 1 : 0, account });
            }
        }
    }
}