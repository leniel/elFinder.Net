using System.Web.Optimization;

namespace elFinder.Net.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/jquery-{version}.js",
                //"~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.validate*"));

            #region elFinder bundles

            bundles.Add(new ScriptBundle("~/Scripts/elfinder").Include(
                "~/Scripts/elfinder/elfinder.full.js"
                //"~/Scripts/elfinder/i18n/elfinder.pt_BR.js"
            ));

            // Themes:
            // Default: "~/Content/elfinder/css/theme.css"
            // Windows 10: "~/Content/elfinder/themes/windows-10/css/theme.css"
            // Bootstrap: "~/Content/elfinder/themes/bootstrap-LibreICONS/css/theme-bootstrap-libreicons-svg.css"
            // Material: "~/Content/elfinder/themes/material/theme.css"
            bundles.Add(new StyleBundle("~/Content/elfinder").Include(
                "~/Content/elfinder/css/elfinder.full.css",
                "~/Content/elfinder/themes/windows-10/css/theme.css"));

            #endregion elFinder bundles

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/jquery-ui")
                .Include("~/Content/themes/base/all.css"));
        }
    }
}