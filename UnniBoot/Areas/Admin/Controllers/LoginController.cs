using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnniBoot.Areas.Admin.Models;
using UnniBoot.Common;

namespace UnniBoot.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(LoginModel model)
        {
            // nếu tất cả thông tin trong form hợp lệ
            if (ModelState.IsValid)
            {
                var userDao = new UserDAO();
                var result = userDao.Login(model.UserName, Encryptor.MD5Hash(model.PassWord), true);// true: thành công.
                // nếu login thành công
                if (result == 1)
                {
                    // tìm ra user
                    var user = userDao.FindUserByUserName(model.UserName);

                    // gán giá trị từ user vừa tìm đc vào session
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    // add thêm GroupId nữa
                    userSession.GroupID = user.GroupID;

                    // ds các quyền
                    var lstCredential = userDao.GetListCredential(model.UserName);

                    // thêm vào session
                    Session[CommonConstant.SESSION_CREDENTIALS] = lstCredential;
                    Session[CommonConstant.USER_SESSION] = userSession;

                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("Index", "Tài khoản không tồn tại");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("Index", "Tài khoản đang bị khóa");
                }
                else if (result == -3)
                {
                    ModelState.AddModelError("Index", "Tài khoản chưa được cấp quyền đăng nhập");
                }
                else
                {
                    ModelState.AddModelError("Index", "Mật khẩu không đúng");
                }
            }
            // trả lại trang Login
            return View("Index");

        }
    }
}