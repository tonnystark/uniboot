using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnniBoot.Controllers
{
    public class ContentController : Controller
    {
        // GET: Content
        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            var model = new ContentDAO().GetAllContent(pageIndex, pageSize);
         
            // tổng số dòng
            int totalRecords = 0;         

            ViewBag.Total = totalRecords;
            ViewBag.Page = pageIndex;

            //chỉ cho tối đa 5 trang thôi
            int maxPage = 5;
            // tổng số trang, mỗi trang có tối đa 2 dònng.
            // làm tròn lại (Ceil: 4.4 => 5)
            int totalPage = (int)Math.Ceiling((double)(totalRecords / pageSize));

            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = pageIndex + 1;
            ViewBag.Prev = pageIndex - 1;
            return View(model);
        }

        public ActionResult GetDetail(long id)
        {
            var model = new ContentDAO().GetContentByID(id);
            ViewBag.Tags = new ContentDAO().GetTagByContent(model.ID);
            return View(model);
        }

        public ActionResult Tag(string tagID, int pageIndex = 1, int pageSize = 10)
        {
            var model = new ContentDAO().GetAllContentByTag(tagID, pageIndex, pageSize);

            // tổng số dòng
            int totalRecords = 0;

            ViewBag.Total = totalRecords;
            ViewBag.Page = pageIndex;
            // lấy ra Tag hiện tại theo id Tag truyền vào
            ViewBag.Tag = new ContentDAO().GetTagByID(tagID);

            //chỉ cho tối đa 5 trang thôi
            int maxPage = 5;
            // tổng số trang, mỗi trang có tối đa 2 dònng.
            // làm tròn lại (Ceil: 4.4 => 5)
            int totalPage = (int)Math.Ceiling((double)(totalRecords / pageSize));

            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = pageIndex + 1;
            ViewBag.Prev = pageIndex - 1;

            return View(model);
        }
    }
}