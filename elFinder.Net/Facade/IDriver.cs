using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;

namespace ElFinder
{
    public interface IDriver
    {
        JsonResult Open(string target, bool tree); 
        JsonResult Init(string target);
        JsonResult Parents(string target);
        JsonResult Tree(string target);
        JsonResult List(string target);
        JsonResult MakeDir(string target, string name);
        JsonResult MakeFile(string target, string name);
        JsonResult Rename(string target, string name);
        JsonResult Remove(IEnumerable<string> targets);
        JsonResult Duplicate(IEnumerable<string> targets);
        JsonResult Get(string target);
        JsonResult Put(string target, string content);        
        JsonResult Paste(string  source, string dest, IEnumerable<string> targets, bool isCut);
        JsonResult Upload(string target, HttpFileCollectionBase targets);
        JsonResult Thumbs(IEnumerable<string> targets);
        JsonResult Dim(string target);
        JsonResult Resize(string target, int width, int height);
        JsonResult Crop(string target, int x, int y, int width, int height);
        JsonResult Rotate(string target, int degree);
        ActionResult File(string target, bool download);
        FullPath ParsePath(string target);
    }
}