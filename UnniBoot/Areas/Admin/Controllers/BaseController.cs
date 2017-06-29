using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UnniBoot.Common;

namespace UnniBoot.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        //public ActionResult Index()
        //{
        //    return View();
        //}
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // lấy ra session của user Admin hiện tại
            var session = (UserLogin)Session[CommonConstant.USER_SESSION];
            // nếu null(chưa đăng nhập) thì redirect sang trang login để đăng nhập
            // nếu đang đăng nhập(session != null) thì không làm gì
            if(session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new {controller = "Login", action = "Index", Area = "Admin"  }));
            }
            //
            base.OnActionExecuting(filterContext);
        }

        protected void SetAlert(string mess, string type)
        {
            TempData["AlertMessage"] = mess;
            if(type == "success")
                TempData["AlertType"] = "alert-success";
            else if(type == "warning")
                TempData["AlertType"] = "alert-warning";
            else if (type == "error")
                TempData["AlertType"] = "alert-danger";
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if(Session[CommonConstant.CurrentCulture] != null)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session[CommonConstant.CurrentCulture].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session[CommonConstant.CurrentCulture].ToString());
            }
            else
            {
                Session[CommonConstant.CurrentCulture] = "vi";
                Thread.CurrentThread.CurrentCulture = new CultureInfo("vi");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi");
            }
        }

        /// <summary>
        /// Change Culture
        /// </summary>
        /// <param name="ddlCulture"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult ChangeCulture(string ddlCulture, string returnUrl)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(ddlCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ddlCulture);

            // gán  culture vào Session
            Session[CommonConstant.CurrentCulture] = ddlCulture;
            return Redirect(returnUrl);
        }
        
    }
}