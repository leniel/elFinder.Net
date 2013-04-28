using ElFinder;
using System.IO;
using System.Web.Mvc;

namespace elFinder.Net.Web.Controllers
{
    public partial class FileController : Controller
    {
        public virtual ActionResult Index(string folder, string subFolder)
        {
            FileSystemDriver driver = new FileSystemDriver();

            var root = new Root(
                    new DirectoryInfo(Server.MapPath("~/Files/" + folder)),
                    "http://" + Request.Url.Authority + "/Files/" + folder)
            {
                IsReadOnly = false, // Can be readonly according to user's membership permission
                Alias = "Files", // Beautiful name given to the root/home folder
                MaxUploadSizeInKb = 500 // Limit imposed to user uploaded file <= 500 KB
            };

            // Was a subfolder selected in Home Index page?
            if (!string.IsNullOrEmpty(subFolder))
            {
                root.StartPath = new DirectoryInfo(Server.MapPath("~/Files/" + folder + "/" + subFolder));
            }

            driver.AddRoot(root);

            var connector = new Connector(driver);

            return connector.Process(this.HttpContext.Request);
        }

        public virtual ActionResult SelectFile(string target)
        {
            FileSystemDriver driver = new FileSystemDriver();

            driver.AddRoot(
                new Root(
                    new DirectoryInfo(Server.MapPath("~/Files")),
                    "http://" + Request.Url.Authority + "/Files") { IsReadOnly = false });

            var connector = new Connector(driver);

            return Json(connector.GetFileByHash(target).FullName);
        }

    }
}
