using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;

namespace UnniBoot
{
    public class HasCredentialAttribute : AuthorizeAttribute
    {
        // quyền của user
        public string RoleID { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // lấy ra user hiện tại
            var session = (UserLogin)HttpContext.Current.Session[Common.CommonConstant.USER_SESSION];
            if (session == null)
                return false;

            // xét quyền đăng nhập
            List<String> privilegeLevels = GetCredentialByLoggedInUser(session.UserName);

            // nếu chứa RoleID hoặc  có quyền Admin
            if (privilegeLevels.Contains(RoleID) || session.GroupID == CommonConstant.ADMIN_GROUP)
            {
                // cấp quyền
                return true;
            }
            return false; // hủy
        }

        List<String> GetCredentialByLoggedInUser(string userName)
        {
            var credentials = (List<String>)HttpContext.Current.Session[Common.CommonConstant.SESSION_CREDENTIALS];
            return credentials;
        }
        // xử lí trang 401 khi không có quyền
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Areas/Admin/Views/Shared/401.cshtml"
            };
        }
    }
}