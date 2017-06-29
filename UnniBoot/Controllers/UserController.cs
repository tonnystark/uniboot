using Model.DAO;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnniBoot.Models;
using BotDetect.Web.Mvc;
using UnniBoot.Common;
using Facebook;
using System.Configuration;
using System.Xml.Linq;

namespace UnniBoot.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        #region Login facebook
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["fbID"],
                client_secret = ConfigurationManager.AppSettings["fbSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["fbID"],
                client_secret = ConfigurationManager.AppSettings["fbSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            if (!String.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;

                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                var user = new User();
                user.Email = email;
                user.UserName = email;
                user.Status = true;
                user.Name = firstname + middlename + lastname;
                user.CreatedDate = DateTime.Now;

                var insertId = new UserDAO().InsertForFacebook(user);
                if (insertId > 0)
                {
                    // gán giá trị từ user vừa tìm đc vào session
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;

                    Session[CommonConstant.USER_SESSION] = userSession;
                }
            }
            return Redirect("/");
        }

        #endregion


        public ActionResult Logout()
        {
            Session[CommonConstant.USER_SESSION] = null;
            return Redirect("/");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var userDao = new UserDAO();
                var result = userDao.Login(model.UserName, Encryptor.MD5Hash(model.PassWord));// true: thành công.
                // nếu login thành công
                if (result == 1)
                {
                    // tìm ra user
                    var user = userDao.FindUserByUserName(model.UserName);
                    // gán giá trị từ user vừa tìm đc vào session
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;

                    Session[CommonConstant.USER_SESSION] = userSession;

                    return Redirect("/");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("Index", "Tài khoản không tồn tại");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("Index", "Tài khoản đang bị khóa");
                }
                else
                {
                    ModelState.AddModelError("Index", "Mật khẩu không đúng");
                }
            }
            return View(model);
        }



        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "RegisterCaptcha", "Mã xác nhận không đúng!")]
        public ActionResult Register(RegisterModel model)
        {
            // nếu mọi thông tin hợp lệ
            if (ModelState.IsValid)
            {
                var dao = new UserDAO();
                // nếu đã có user
                if (dao.CheckUserName(model.UserName) || dao.CheckEmail(model.Email))
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc Email đã đăng ký");
                }
                // tạo user
                else
                {
                    var user = new User();
                    user.Name = model.Name;
                    user.UserName = model.UserName;
                    // mã hóa MD5
                    user.Password = Encryptor.MD5Hash(model.PassWord);
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Address = model.Address;
                    user.CreatedDate = DateTime.Now;
                    // sẽ gửi 1 link kích hoạt qua mail
                    // nếu nhấn vào, sẽ đổi Status => true(kích hoạt)
                    user.Status = true;
                    if (!String.IsNullOrEmpty(model.ProvinceID.ToString()))
                    {
                        user.ProvinceID = model.ProvinceID;                      
                    }
                    if (!String.IsNullOrEmpty(model.DistrictID.ToString()))
                    {
                        user.DistrictID = model.DistrictID;
                    }
                        var result = dao.Insert(user);
                    // thêm thành công
                    if (result > 0)
                    {
                        ViewBag.Success = "Đăng ký thành công";
                        // reset lại
                        model = new RegisterModel();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công");
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Load tỉnh thành
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadProvince()
        {
            // lấy ra nội dung xml
            var xmlDoc = XDocument.Load(Server.MapPath(@"~\Assets\client\data\Provinces_Data.xml"));
            // lấy ra các phần tử có giá trị của attribute là province
            var xElements = xmlDoc.Descendants("Item").Where(x => x.Attribute("type").Value == "province");

            var lstProvince = new List<ProvinceModel>();
            foreach (var item in xElements)
            {
                ProvinceModel p = new ProvinceModel();
                p.ID = int.Parse(item.Attribute("id").Value);
                p.Name = item.Attribute("value").Value;

                lstProvince.Add(p);
            }

            return Json(new
            {
                // trả về 1 data chứa ds các tỉnh thành
                data = lstProvince,
                status = true
            });
        }

        /// <summary>
        /// Load quận/ huyện
        /// </summary>
        /// <param name="provinceID"></param>
        /// <returns></returns>
        public JsonResult LoadDistrict(int provinceID)
        {
            var xmlDoc = XDocument.Load(Server.MapPath(@"~\Assets\client\data\Provinces_Data.xml"));
            var xElements = xmlDoc.Descendants("Item").Where(x => x.Attribute("type").Value == "province" && x.Attribute("id").Value == provinceID.ToString());

            var lstDistrict = new List<DistrictModel>();
            foreach (var item in xElements.Descendants("Item").Where(x => x.Attribute("type").Value == "district"))
            {
                DistrictModel p = new DistrictModel();
                p.ID = int.Parse(item.Attribute("id").Value);
                p.Name = item.Attribute("value").Value;
                p.ProvinceID = provinceID;

                lstDistrict.Add(p);
            }

            return Json(new
            {
                // trả về 1 data chứa ds các tỉnh thành
                data = lstDistrict,
                status = true
            });
        }

        /// <summary>
        /// Load xã
        /// </summary>
        /// <param name="districtID"></param>
        /// <returns></returns>
        public JsonResult LoadWard(int districtID)
        {
            var xmlDoc = XDocument.Load(Server.MapPath(@"~\Assets\client\data\Provinces_Data.xml"));
            var xElements = xmlDoc.Descendants("Item").Where(x => x.Attribute("type").Value == "province").Descendants("Item").Where(x => x.Attribute("type").Value == "district" && x.Attribute("id").Value == districtID.ToString());

            var lstDistrict = new List<WardModel>();
            foreach (var item in xElements.Descendants("Item").Where(x => x.Attribute("type").Value == "precinct"))
            {
               WardModel p = new WardModel();
                
                p.ID = int.Parse(item.Attribute("id").Value);
                p.Name = item.Attribute("value").Value;
                p.DistrictID = districtID;

                lstDistrict.Add(p);
            }

            return Json(new
            {
                // trả về 1 data chứa ds các tỉnh thành
                data = lstDistrict,
                status = true
            });
        }

    }
}