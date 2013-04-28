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
                             "~/Content/elfinder/js/elfinder.full.js"
                //"~/Content/elfinder/js/i18n/elfinder.pt_BR.js"
                             ));

            bundles.Add(new StyleBundle("~/Content/elfinder").Include(
                            "~/Content/elfinder/css/elfinder.full.css",
                            "~/Content/elfinder/css/theme.css"));

            #endregion

            bundles.Add(new StyleBundle("~/Content/css").Include(
                                        "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/ui-lightness/css").Include(
                                        "~/Content/themes/ui-lightness/jquery.ui.all.css"));
        }
    }
}