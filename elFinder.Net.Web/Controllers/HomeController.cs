using AttributeRouting.Web.Mvc;
using elFinder.Net.Web.ViewModels;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace elFinder.Net.Web.Controllers
{
    public partial class HomeController : Controller
    {
        [GET("")]
        public virtual ActionResult Index()
        {
            var di = new DirectoryInfo(Server.MapPath("~/Files/MyFolder"));
            // Enumerating all 1st level directories of a given root folder (MyFolder in this case) and retrieving the folders names.
            var folders = di.GetDirectories().ToList().Select(d => d.Name);

            return View(folders);
        }

        [GET("FileManager/{subFolder?}")]
        public virtual ActionResult Files(string subFolder)
        {        // FileViewModel contains the root MyFolder and the selected subfolder if any
            var model = new FileViewModel { Folder = "MyFolder", SubFolder = subFolder };

            return View(model);
        }
        
        public ActionResult TinyMceFiles(string subFolder)
        {        
            var model = new FileViewModel { Folder = "MyFolder", SubFolder = subFolder };
            return View(model);
        }

        public ActionResult CkEditorFiles(string subFolder)
        {
            var model = new FileViewModel { Folder = "MyFolder", SubFolder = subFolder };
            return View(model);
        }

        #region File dialog
        public ActionResult FileBrowser(string subFolder, string caller, string langCode, string type, string fn, string CKEditorFuncNum)
        {            
            ViewBag.Caller = caller;
            ViewBag.LangCode = langCode;
            ViewBag.Type = type;
            ViewBag.Callbackfn = fn;
            ViewBag.CKEditorFuncNum = CKEditorFuncNum;
            var model = new FileViewModel { Folder = "MyFolder", SubFolder = subFolder };
            return View(model);
        }
        #endregion
    }
}
