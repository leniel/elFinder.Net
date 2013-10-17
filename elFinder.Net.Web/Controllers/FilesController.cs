using System.Web.Mvc;
using System.IO;

namespace ElFinder.Sample.Controllers
{
    public class FilesController : Controller
    {
        private Connector _connector;

        public Connector Connector
        {
            get
            {
                if (_connector == null)
                {
                    FileSystemDriver driver = new FileSystemDriver();
                    DirectoryInfo thumbsStorage = new DirectoryInfo(Server.MapPath("~/Files"));
                    driver.AddRoot(new Root(new DirectoryInfo(@"C:\Program Files"))
                    {
                        IsLocked = true,
                        IsReadOnly = true,
                        IsShowOnly = true,
                        ThumbnailsStorage = thumbsStorage,
                        ThumbnailsUrl = "Thumbnails/"
                    });
                    driver.AddRoot(new Root(new DirectoryInfo(Server.MapPath("~/Files")), "/Files/")
                    {
                        Alias = "My documents",
                        StartPath = new DirectoryInfo(Server.MapPath("~/Files/новая папка")),
                        ThumbnailsStorage = thumbsStorage,
                        MaxUploadSizeInMb = 2.2,
                        ThumbnailsUrl = "Thumbnails/"
                    });
                    _connector = new Connector(driver); 
                }
                return _connector;
            }
        }
        public ActionResult Index()
        {
            return Connector.Process(this.HttpContext.Request);
        }

        public ActionResult SelectFile(string target)
        {
            return Json(Connector.GetFileByHash(target).FullName);
        }

        public ActionResult Thumbs(string tmb)
        {
            return Connector.GetThumbnail(Request, Response, tmb);
        }
    }
}
