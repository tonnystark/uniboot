using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnniBoot.Models
{
    public class RegisterModel
    {
        [Key]
        public long ID { get; set; }
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải chứa ít nhất 6 kí tự")]
        public string PassWord { get; set; }
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("PassWord", ErrorMessage = "Mật khẩu nhập lại không đúng")]
        public string ConfirmPassWord { get; set; }
        [Display(Name = "Họ tên")]
        public string Name { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập vào Email")]
        public string Email { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Tỉnh/ Thành")]
        public int ProvinceID { get; set; }

        [Display(Name = "Quận/ Huyện")]
        public int DistrictID { get; set; }

        [Display(Name = "Xã")]
        public int WardID { get; set; }
    }
}