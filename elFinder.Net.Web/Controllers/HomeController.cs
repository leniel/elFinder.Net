﻿using System.IO;
using System.Linq;
using System.Web.Mvc;
using elFinder.Net.Web.ViewModels;

namespace elFinder.Net.Web.Controllers
{
    [RouteArea("")]
    public partial class HomeController : Controller
    {
        [Route("")]
        public virtual ActionResult Index()
        {
            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Files/MyFolder"));
            // Enumerating all 1st level directories of a given root folder (MyFolder in this case) and retrieving the folders names.
            var folders = di.GetDirectories().ToList().Select(d => d.Name);

            return View(folders);
        }

        [Route("file-manager/{subFolder?}")]
        public virtual ActionResult Files(string subFolder)
        {        // FileViewModel contains the root MyFolder and the selected subfolder if any
            FileViewModel model = new FileViewModel() { Folder = "MyFolder", SubFolder = subFolder };

            return View(model);
        }
    }
}