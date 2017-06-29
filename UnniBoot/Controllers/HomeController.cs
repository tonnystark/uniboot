using Model.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnniBoot.Models;

namespace UnniBoot.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // tạo 1 viewBag chứa các slide
            ViewBag.Slides = new SlideDAO().ListAllSlide();
            ViewBag.NewProducts = new ProductDAO().ListNewProducts(4);
            ViewBag.FeaturedProducts = new ProductDAO().ListFeaturedProduct(4);

            ViewBag.Title = ConfigurationManager.AppSettings["HomeTitle"];
            ViewBag.HomeKeywords = ConfigurationManager.AppSettings["HomeKeywords"];
            ViewBag.HomeDescriptions = ConfigurationManager.AppSettings["HomeDescriptions"];

            return View();
        }

        // thuộc tính ChildActionOnly chỉ dành cho partial view
        [ChildActionOnly]      
        public ActionResult MainMenu()
        {
            var model = new MenuDAO().LstByTypeId(1);
            return PartialView(model);
        }

        /// <summary>
        /// Trả về 1 giỏ hàng
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        
        public PartialViewResult HeaderCart()
        {
            var cart = Session[Common.CommonConstant.CartSession];

            var listItem = new List<CartItem>();
            if (cart != null)
            {
                listItem = (List<CartItem>)cart;
            }
           
            return PartialView(listItem);
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            var model = new MenuDAO().LstByTypeId(2);
            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FooterDAO().GetFooter();
            return PartialView(model);
        }


    }
}