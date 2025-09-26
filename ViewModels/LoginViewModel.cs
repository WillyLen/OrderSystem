using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models.ViewModels
{
    public class LoginViewModel
    {
        public string SessionId    { get; set; } // 對應登入動作的sessionId
        public string Account      { get; set; } // 帳號
        public string Password     { get; set; } // 密碼
        public string ErrorMessage { get; set; } // 錯誤訊息
    }
}
