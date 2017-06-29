using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UnniBoot
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            routes.MapRoute(
               name: "All Product",
               url: "san-pham",
               defaults: new { controller = "Product", action = "GetAllProduct", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
               );

            routes.MapRoute(
                name: "Tag",
                url: "tag/{tagID}",
                defaults: new { controller = "Content", action = "Tag", id = UrlParameter.Optional },
                namespaces: new[] { "UnniBoot.Controllers" }
                );


            routes.MapRoute(
name: "News Detail",
url: "tin-tuc/{metatitle}-{id}",
defaults: new { controller = "Content", action = "GetDetail", id = UrlParameter.Optional },
namespaces: new[] { "UnniBoot.Controllers" }
);

            routes.MapRoute(
   name: "News",
   url: "tin-tuc",
   defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional },
  namespaces: new[] { "UnniBoot.Controllers" }
);

            routes.MapRoute(
     name: "Search",
     url: "tim-kiem",
     defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
    namespaces: new[] { "UnniBoot.Controllers" }
 );

            routes.MapRoute(
        name: "Login",
        url: "dang-nhap",
        defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional },
       namespaces: new[] { "UnniBoot.Controllers" }
    );

            routes.MapRoute(
           name: "Register",
           url: "dang-ky",
           defaults: new { controller = "User", action = "Register", id = UrlParameter.Optional },
          namespaces: new[] { "UnniBoot.Controllers" }
       );

            routes.MapRoute(
             name: "Contact",
             url: "lien-he",
             defaults: new { controller = "Contact", action = "Index", id = UrlParameter.Optional },
            namespaces: new[] { "UnniBoot.Controllers" }
         );

            routes.MapRoute(
              name: "Payment Sucess",
              url: "hoan-thanh",
              defaults: new { controller = "Cart", action = "Success", id = UrlParameter.Optional },
             namespaces: new[] { "UnniBoot.Controllers" }
          );


            routes.MapRoute(
                name: "Payment",
                url: "thanh-toan",
                defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
            );

            routes.MapRoute(
                name: "Cart",
                url: "gio-hang",
                defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
            );

            routes.MapRoute(
                name: "Add Cart",
                url: "them-vao-gio",
                defaults: new { controller = "Cart", action = "AddItem", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
            );

            routes.MapRoute(
                name: "About",
                url: "gioi-thieu",
                defaults: new { controller = "About", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
            );

            routes.MapRoute(
                name: "Product Category",
                url: "san-pham/{metatitle}-{id}",
                defaults: new { controller = "Product", action = "Category", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
            );

            routes.MapRoute(
                name: "Product Detail",
                url: "chi-tiet/{metatitle}-{id}",
                defaults: new { controller = "Product", action = "DetailProduct", id = UrlParameter.Optional },
               namespaces: new[] { "UnniBoot.Controllers" }
            );


            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "UnniBoot.Controllers" }
           );
        }
    }
}
