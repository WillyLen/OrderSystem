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
        private string connectionString = ConfigurationManager.ConnectionStrings["OET8con"].ConnectionString;

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