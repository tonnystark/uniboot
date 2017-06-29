using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.EF;
using Model.DAO;
using UnniBoot.Common;
using PagedList;

namespace UnniBoot.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        // GET: Admin/User
        // trang mặc định khi tạo user xong
        /// <summary>
        /// Trả về danh sách các User đã phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        [HasCredential(RoleID = "VIEW_USER")]
        public ActionResult Index(string strSearch, int page = 1, int pageSize = 5)
        {
            var userDao = new UserDAO();
            // hỗ trợ phân trang
            var model = userDao.GetAllUserPagging(strSearch, page, pageSize);
            // lưu lại chuỗi vừa search
            ViewBag.strSearch = strSearch;
            SetViewBag();
            return View(model);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult CreateUser()
        {
            SetViewBag();
            return View();
        }

        [HttpPost] // post lên Server
                   // Thực hiện thêm vào 1 User
        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                SetViewBag();
                var dao = new UserDAO();

                var encryptPass = Encryptor.MD5Hash(user.Password);
                user.Password = encryptPass;

                long id = dao.Insert(user);
                // nếu thêm đc thì trả ra danh sách user
                if (id > 0)
                {
                    SetAlert("Thêm user thành công", "success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm User không thành công");
                }
            }
            // nếu không thêm đc thì trả lại trang này để thêm lại User
            return View("Index");
        }

        // thực hiện Edit User
        [HttpGet]
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult EditUser(long id)
        {
            var user = new UserDAO().FindUserById(id);
            SetViewBag(user.GroupID);
            // trả ra 1 user để edit
            return View(user);
        }

        [HasCredential(RoleID = "EDIT_USER")]
        [HttpPost] // post lên Server
        // Thực hiện edit 1 User
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDAO();

                if (!String.IsNullOrEmpty(user.Password))
                {
                    var encryptPass = Encryptor.MD5Hash(user.Password);
                    user.Password = encryptPass;
                }
                SetViewBag(user.GroupID);
                bool update = dao.UpdateUser(user);              
                // nếu update đc thì trả ra danh sách user
                if (update)
                {
                    SetAlert("Edit user thành công", "success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Edit User không thành công");
                }
            }
            // nếu không thêm đc thì trả lại trang này để thêm lại User
            return View("Index");
        }
        /// <summary>
        /// Xóa một User tại Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HasCredential(RoleID = "DELETE_USER")]
        [HttpDelete]
        public ActionResult DeleteUser(long id)
        {
            new UserDAO().DeleteUser(id);
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "EDIT_USER")]
        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new UserDAO().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        public ActionResult Logout()
        {
            Session[CommonConstant.USER_SESSION] = null;
            return RedirectToAction("Index", "Home");
        }

        public void SetViewBag(string selectedId = null)
        {
            var dao = new UserDAO();
            ViewBag.GroupID = new SelectList(dao.GetGroupUser(), "ID", "Name", selectedId);
        }
    }
}


