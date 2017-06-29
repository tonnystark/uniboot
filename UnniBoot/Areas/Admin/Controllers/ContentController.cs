using Model.DAO;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnniBoot.Common;

namespace UnniBoot.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        // GET: Admin/Content      
        [HasCredential(RoleID = "VIEW_CONTENT")]
        public ActionResult Index(string strSearch, int page = 1, int pageSize = 5)
        {
            var contentDao = new ContentDAO();
            // hỗ trợ phân trang
            var model = contentDao.GetAllContentPagging(strSearch, page, pageSize);
            // lưu lại chuỗi vừa search
            ViewBag.strSearch = strSearch;

            return View(model);
        }
        [HasCredential(RoleID = "EDIT_CONTENT")]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var dao = new ContentDAO();
            var content = dao.GetContentByID(id);

            SetViewBag(content.CategoryID);

            return View(content);
        }
        [HasCredential(RoleID = "EDIT_CONTENT")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Content model)
        {
            if (ModelState.IsValid)
            {

                long update = new ContentDAO().EditContent(model);
                // nếu update đc thì trả ra danh sách user
                if (update > 0)
                {
                    SetAlert("Edit tin tức thành công", "success");
                    return RedirectToAction("Index", "Content");
                }
                else
                {
                    ModelState.AddModelError("", "Edit tin tức không thành công");
                }
            }
            SetViewBag(model.CategoryID);
            return View();
        }

        [HasCredential(RoleID = "ADD_CONTENT")]
        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }
        [HasCredential(RoleID = "ADD_CONTENT")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Content model )
        { 
            if(ModelState.IsValid)
            {
                // lấy ra user
                var session = (UserLogin)Session[CommonConstant.USER_SESSION];
                model.CreatedBy = session.UserName;
                // set culture
                var culture = Session[CommonConstant.CurrentCulture];
                model.Language = culture.ToString();

                new ContentDAO().CreateContent(model);
                return RedirectToAction("Index");
            }
            SetViewBag();
            return View();
        }
        /// <summary>
        /// Chứa danh sách các Category
        /// ViewBag.CategoryID phải trùng với CategoryID trong Model
        /// </summary>
        /// <param name="selectedId"></param>

        public void SetViewBag(long? selectedId = null)
        {
            var dao = new CategoryDAO();
            ViewBag.CategoryID = new SelectList(dao.ListAllCategory(), "ID", "Name", selectedId);
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ContentDAO().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
    }
}