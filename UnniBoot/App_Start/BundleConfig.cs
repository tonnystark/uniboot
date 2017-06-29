using System.Web;
using System.Web.Optimization;

namespace UnniBoot
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/coreJS").Include(
                        "~/Assets/client/js/jquery-2.2.4.min.js",
                        "~/Assets/client/js/bootstrap.min.js",
                        "~/Assets/client/js/jquery-ui.js",
                        "~/Assets/client/js/move-top.js",
                        "~/Assets/client/js/easing.js",
                        "~/Assets/client/js/startstop-slider.js"
                        ));
               

            bundles.Add(new ScriptBundle("~/bundles/controller").Include(
                      "~/Assets/client/js/controller/baseController.js"));

            bundles.Add(new StyleBundle("~/bundles/coreCSS").Include(
                      "~/Assets/client/css/bootstrap-theme.css",
                      "~/Assets/client/css/bootstrap.css",
                      "~/Assets/client/css/font-awesome.min.css",
                      "~/Assets/client/css/style.css",
                      "~/Assets/client/css/slider.css",
                      "~/Assets/client/css/jquery-ui.css"
                      ));
            BundleTable.EnableOptimizations = true;

        }
    }
}
