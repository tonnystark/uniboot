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
    public class CategoryController : BaseController
    {
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                var currentCulture = Session[CommonConstant.CurrentCulture];
                model.Language = currentCulture.ToString();

                var id = new CategoryDAO().InsertCategory(model);
                if(id > 0)
                {
                    SetAlert("Thêm user thành công", "success");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", StaticResource.Resource.InsertCategoryFailed);
                }
            }
            return View(model);
        }
    }
}