using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnniBoot.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Bạn phải nhập vào UserName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập vào PassWord")]
        public string PassWord { get; set; }
        public bool RememberMe { get; set; }
    }
}