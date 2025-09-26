using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Email         { get; set; } // Email
        public string Account       { get; set; } // 帳號
        public string Password      { get; set; } // 密碼
        public string SessionId     { get; set; } // 對應登入動作的sessionId
        public string ErrorMessage  { get; set; } // 註冊失敗的錯誤訊息
        public bool RegisterSuccess { get; set; } // 註冊是否成功(true或false)
    }
}
