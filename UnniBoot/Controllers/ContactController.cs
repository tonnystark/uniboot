using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.DAO;
using Model.EF;

namespace UnniBoot.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            var model = new ContactDAO().GetActiveContact();
            return View(model);
        }
        [HttpPost]
        /// <summary>
        /// Chèn vào 1 feedBack của khách
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mobile"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Send(string name, string mobile, string address, string email, string content)
        {
            var fb = new Feedback();
            fb.Name = name;
            fb.Email = email;
            fb.Phone = mobile;
            fb.CreatedDate = DateTime.Now;
            fb.Content = content;
            fb.Address = address;

            var id = new ContactDAO().InsertFeedBack(fb);
            if (id > 0)
                return Json(new
                {
                    status = true
                });
            else
                return Json(new
                {
                    status = false
                });
        }
    }
}