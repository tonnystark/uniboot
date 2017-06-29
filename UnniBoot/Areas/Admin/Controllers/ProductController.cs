using Model.DAO;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using UnniBoot.Common;

namespace UnniBoot.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Admin/Product

        public ActionResult Index(string strSearch, int page = 1, int pageSize = 5)
        {
            var dao = new ProductDAO();
            var model = dao.GetAllProductByKeyword(strSearch, page, pageSize);

            ViewBag.SearchString = strSearch;

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session[CommonConstant.USER_SESSION];
                model.CreatedBy = session.UserName;
                model.CreatedDate = DateTime.Now;

                long id = new ProductDAO().CreateProduct(model);
                if (id > 0)
                {
                    SetAlert("Thêm sản phẩm thành công", "success");
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError("", "Thêm sản phẩm mới thất bại");
            }

            SetViewBag();
            return View(model);
        }

        public void SetViewBag(long? selectedId = null)
        {
            var dao = new ProductDAO();
            ViewBag.CategoryID = new SelectList(dao.GetProductCategory(), "ID", "Name", selectedId);
        }

        [HttpDelete]
        public ActionResult DeleteProduct(long id)
        {
            new ProductDAO().DeleteProduct(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var dao = new ProductDAO();
            var product = dao.GetProducById(id);

            SetViewBag(product.ID);
            return View(product);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                SetViewBag(model.ID);
                long id = new ProductDAO().EditProduct(model);

                if (id > 0)
                {
                    SetAlert("Edit sản phẩm thành công", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Edit sản phẩm thất bại");
                }

            }

            return View();
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ProductDAO().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        public JsonResult SaveImages(long productID, string images)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // chuyển từ Json string  sang dạng List<string>
            var lstImage = serializer.Deserialize<List<string>>(images);
            // Tạo ra 1 thẻ XML Root <Images>
            XElement xElement = new XElement("Images");
            // Add các thẻ Image con vào Root
            foreach (var item in lstImage)
            {
                // cắt đường dẫn cho ảnh
                string str = item.Substring(FindStringIndex(item));
                xElement.Add(new XElement("Image", str));
            }
            bool update = new ProductDAO().UpdateImages(productID, xElement.ToString());

            return Json(new
            {
                status = update
            });
        }

        public JsonResult LoadImages(long id)
        {          
            var product = new ProductDAO().ProductDetail(id);
            var images = product.MoreImages;
            // nếu đã có ảnh trước đó
            if (images != null)
            {
                // chuyển chuỗi link image trong db sang dạng XML
                XElement xe = XElement.Parse(images);
                List<string> lstImages = (from img in xe.Elements("Image")
                                          select img.Value).ToList();
                return Json(new
                {
                    status = true,
                    data = lstImages
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = false,               
            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Tìm ra index của vị trí dấu '// thứ 3 để cắt
        /// Cấu trúc: http://localhost:29488/Data/img.png
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        int FindStringIndex(string str)
        {
            int count = 0, idx = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '/')
                    count++;
                if (count == 3)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
    }
}