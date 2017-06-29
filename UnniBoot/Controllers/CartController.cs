using Common;
using Model.DAO;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UnniBoot.Models;

namespace UnniBoot.Controllers
{
    public class CartController : Controller
    {
        private const string CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var listItem = new List<CartItem>();
            if (cart != null)
            {
                listItem = (List<CartItem>)cart;
            }
            return View(listItem);
        }

        /// <summary>
        /// Xóa 1 item trong giỏ
        /// Ứng với productId truyền vào
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteItem(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            // xóa tại id truyền vào ứng vs Product
            sessionCart.RemoveAll(x => x.Product.ID == id);

            // gán lại session
            Session[CartSession] = sessionCart;

            return Json(new
            {
                status = true
            });
        }

        /// <summary>
        /// Xóa tất cả trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;

            return Json(new
            {
                status = true
            });
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm
        /// </summary>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                // kiểm tra nếu thông tin khớp nhau
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    // cập nhật lại số lượng
                    item.Quantity = jsonItem.Quantity;
                }
            }
            return Json(new
            {
                status = true
            });
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public ActionResult AddItem(long productId, int quantity)
        {
            // lấy ra 1 product từ productID
            var product = new ProductDAO().ProductDetail(productId);

            var cart = Session[CartSession];
            // giỏ có hàng
            if (cart != null)
            {
                var lstItem = (List<CartItem>)cart;
                // nếu cùng productId (cùng sản phẩm) => tăng số lượng
                if (lstItem.Exists(x => x.Product.ID == productId))
                {
                    foreach (var item in lstItem)
                    {
                        if (item.Product.ID == productId)
                        {
                            // tăng số lượng
                            item.Quantity += quantity;
                        }
                    }
                }
                else // thêm sản phẩm mới
                {
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;

                    // thêm item
                    lstItem.Add(item);
                }

                Session[CartSession] = lstItem;
            }
            else
            {
                // tạo mới cartItem
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;

                var lstItem = new List<CartItem>();
                lstItem.Add(item);

                // thêm vào Session
                Session[CartSession] = lstItem;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[CartSession];
            var listItem = new List<CartItem>();
            if (cart != null)
            {
                listItem = (List<CartItem>)cart;
            }

            return View(listItem);
        }

        [HttpPost]
        public ActionResult Payment(string shipName, string mobile, string address, string email)
        {
            var order = new Order();
            order.CreatedDate = DateTime.Now;
            order.ShipName = shipName;
            order.ShipMobile = mobile;
            order.ShipAddress = address;
            order.ShipEmail = email;

            // tổng tiền
            decimal Total = 0;

            try
            {
                // thêm vào 1 order, trả về id của order vừa thêm
                var id = new OrderDAO().Insert(order);
                var sessionCart = (List<CartItem>)Session[CartSession];
                var detailDAO = new OrderDetailDAO();
                // lặp qua tất cả các mặt hàng lấy ra detail
                foreach (var item in sessionCart)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.OrderID = id;
                    orderDetail.Price = item.Product.Price;
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.Quantity = item.Quantity;

                    detailDAO.Insert(orderDetail);

                    Total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                }

                // lấy ra content của trang  template đã tạo
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/client/template/newOrder.html"));

                // thay đổi lại các giá trị cho nó
                content = content.Replace("{{Phone}}", mobile);
                content = content.Replace("{{CustomerName}}", shipName);
                content = content.Replace("{{Email}}", email);
                content = content.Replace("{{Address}}", address);
                content = content.Replace("{{Total}}", Total.ToString());
                var toEmail = ConfigurationManager.AppSettings["ToEmail"].ToString();

                new MailHelper().SendMail(toEmail, "Đơn hàng mới", content);
            }
            catch (Exception ex)
            {
                return Redirect("/error");
            }
            return Redirect("/hoan-thanh");
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}