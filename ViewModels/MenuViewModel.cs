using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderSystem.Models.ViewModels
{
    public class MenuViewModel
    {
        public string account { get; set; } // 帳號
        public string role    { get; set; } // 角色權限(Admin / User)
    }
}