using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnniBoot.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            var model = new ProductCategoriesDAO().ListAllProductCategory();
            return PartialView(model);
        }
        public ActionResult Category(long id, int pageIndex = 1, int pageSize = 1)
        {
            var category = new CategoryDAO().ViewDetail(id);
            ViewBag.Category = category;
            // tổng số dòng
            int totalRecords = 0;
            var model = new ProductDAO().lstByCategoryId(id, ref totalRecords, pageIndex, pageSize);

            ViewBag.Total = totalRecords;
            ViewBag.Page = pageIndex;

            //chỉ cho tối đa 5 trang thôi
            int maxPage = 5;
            // tổng số trang, mỗi trang có tối đa 2 dònng.
            // làm tròn lại
            int totalPage = (int)Math.Ceiling((double)(totalRecords / pageSize));

            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = pageIndex + 1;
            ViewBag.Prev = pageIndex - 1;


            return View(model);
        }

        public ActionResult DetailProduct(long id)
        {
            var product = new ProductDAO().ProductDetail(id);
            ViewBag.Category = new ProductCategoriesDAO().ProductCategoriesViewDetail(product.CategoryID);
            ViewBag.RelatedProducts = new ProductDAO().ListRelatedProducts(id);
            return View(product);
        }

        /// <summary>
        /// Trả về 1 jSon chứa data là danh sách các Name theo keyword truyền vào
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public JsonResult ListName(string keyword)
        {
            var data = new ProductDAO().ListNameProduct(keyword);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Trả về một View mới chứa ds các sản phẩm cần tìm
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Search(string keyword, int pageIndex = 1, int pageSize = 1)
        {
            ViewBag.KeyWord = keyword;

            // tổng số dòng
            int totalRecords = 0;
            var model = new ProductDAO().Search(keyword, ref totalRecords, pageIndex, pageSize);

            ViewBag.Total = totalRecords;
            ViewBag.Page = pageIndex;

            //chỉ cho tối đa 5 trang thôi
            int maxPage = 5;
            // tổng số trang, mỗi trang có tối đa 2 dònng.
            // làm tròn lại
            int totalPage = (int)Math.Ceiling((double)(totalRecords / pageSize));

            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = pageIndex + 1;
            ViewBag.Prev = pageIndex - 1;

            return View(model);
        }

        public ActionResult GetAllProduct()
        {
            var dao = new ProductDAO();
            var model = dao.GetAllProduct();
            return View(model);
        }
    }
}