using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

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
        ActionResult File(string target, bool download);
        FullPath ParsePath(string target);
    }
}