using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using ElFinder.DTO;
using System.IO;

namespace ElFinder
{
    /// <summary>
    /// Represents a connector which process elFinder request
    /// </summary>
    public class Connector
    {
        private IDriver _driver;
        /// <summary>
        /// Initialize new instance of ElFinder.Connector
        /// </summary>
        /// <param name="driver">Driver to process request</param>
        public Connector(IDriver driver)
        {
            _driver = driver;             
        }

        /// <summary>
        /// Process elFinder request
        /// </summary>
        /// <param name="request">Request from elFinder</param>
        /// <returns>Json response, which must be sent to elfinder</returns>
        public ActionResult Process(HttpRequestBase request)
        {
            NameValueCollection parameters = request.QueryString.Count > 0 ? request.QueryString : request.Form;
            string cmdName = parameters["cmd"];
            if (string.IsNullOrEmpty(cmdName))
                return Error.CommandNotFound();

            string target = parameters["target"];
            if (target != null && target.ToLower() == "null")
                target = null;
            switch (cmdName)
            {
                case "open":
                    if (!string.IsNullOrEmpty(parameters["init"]) && parameters["init"] == "1")
                    {
                        return _driver.Init(target);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(target))
                            return Error.MissedParameter(cmdName);
                        return _driver.Open(target, !string.IsNullOrEmpty(parameters["tree"]) && parameters["tree"] == "1");
                    }
                case "file":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    return _driver.File(target, !string.IsNullOrEmpty(parameters["download"]) && parameters["download"] == "1");
                case "tree":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    return _driver.Tree(target);
                case "parents":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    return _driver.Parents(target);
                case "mkdir":
                    {
                        if (string.IsNullOrEmpty(target))
                            return Error.MissedParameter(cmdName);
                        string name = parameters["name"];

                        if (string.IsNullOrEmpty(name))
                            return Error.MissedParameter("name");
                        return _driver.MakeDir(target, name);
                    }
                case "mkfile":
                    {
                        if (string.IsNullOrEmpty(target))
                            return Error.MissedParameter(cmdName);
                        string name = parameters["name"];

                        if (string.IsNullOrEmpty(name))
                            return Error.MissedParameter("name");
                        return _driver.MakeFile(target, name);
                    }
                case "rename":
                    {
                        if (string.IsNullOrEmpty(target))
                            return Error.MissedParameter(cmdName);
                        string name = parameters["name"];

                        if (string.IsNullOrEmpty(name))
                            return Error.MissedParameter("name");
                        return _driver.Rename(target, name);
                    }
                case "rm":
                    {
                        IEnumerable<string> targets = GetTargetsArray(request);
                        if (targets == null)
                            Error.MissedParameter("targets");
                        return _driver.Remove(targets);
                    }
                case "ls":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    return _driver.List(target);
                case "get":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    return _driver.Get(target);
                case "put":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    string content = parameters["content"];

                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter("content");
                    return _driver.Put(target, content);
                case "paste":
                    {
                        IEnumerable<string> targets = GetTargetsArray(request);
                        if (targets == null)
                            Error.MissedParameter("targets");
                        string src = parameters["src"];
                        if (string.IsNullOrEmpty(src))
                            return Error.MissedParameter("src");

                        string dst = parameters["dst"];
                        if (string.IsNullOrEmpty(src))
                            return Error.MissedParameter("dst");

                        return _driver.Paste(src, dst, targets, !string.IsNullOrEmpty(parameters["cut"]) && parameters["cut"] == "1");
                    }
                case "upload":
                    if (string.IsNullOrEmpty(target))
                        return Error.MissedParameter(cmdName);
                    return _driver.Upload(target, request.Files);
                case "duplicate":
                    {
                        IEnumerable<string> targets = GetTargetsArray(request);
                        if(targets == null)
                            Error.MissedParameter("targets");
                        return _driver.Duplicate(targets);
                    }
                case "tmb":
                    {
                        IEnumerable<string> targets = GetTargetsArray(request);
                        if (targets == null)
                            Error.MissedParameter("targets");
                        return _driver.Thumbs(targets);
                    }
                case "dim":
                    {
                        if (string.IsNullOrEmpty(target))
                            return Error.MissedParameter(cmdName);
                        return _driver.Dim(target);
                    }
                case "resize":
                    {
                        if (string.IsNullOrEmpty(target))
                            return Error.MissedParameter(cmdName);                        
                        switch (parameters["mode"])
                        {
                            case "resize":
                                return _driver.Resize(target, int.Parse(parameters["width"]), int.Parse(parameters["height"]));
                            case "crop":
                                return _driver.Crop(target, int.Parse(parameters["x"]), int.Parse(parameters["y"]), int.Parse(parameters["width"]), int.Parse(parameters["height"]));
                            case "rotate":
                                return _driver.Rotate(target, int.Parse(parameters["degree"]));
                            default:
                                break;
                        }
                        return Error.CommandNotFound();
                    }
                default:
                    return Error.CommandNotFound();
            }
        }

        /// <summary>
        /// Get actual filesystem path by hash
        /// </summary>
        /// <param name="hash">Hash of file or directory</param>
        public FileSystemInfo GetFileByHash(string hash)
        {
            FullPath path = _driver.ParsePath(hash);
            return !path.IsDirectoty ? (FileSystemInfo)path.File : (FileSystemInfo)path.Directory;
        }

        public ActionResult GetThumbnail(HttpRequestBase request, HttpResponseBase response, string hash)
        {
            string thumbHash = hash;
            if (thumbHash != null)
            {
                FullPath path = _driver.ParsePath(thumbHash);
                if (!path.IsDirectoty && path.Root.CanCreateThumbnail(path.File))
                {
                    if (!HttpCacheHelper.IsFileFromCache(path.File, request, response))
                    {
                        ImageWithMime thumb = path.Root.GenerateThumbnail(path);
                        return new FileStreamResult(thumb.ImageStream, thumb.Mime);                        
                    }
                    else
                    {
                        response.ContentType = Helper.GetMimeType(path.Root.PicturesEditor.ConvertThumbnailExtension(path.File.Extension));
                        response.End();
                    }
                }
            }
            return new EmptyResult();
        }

        private IEnumerable<string> GetTargetsArray(HttpRequestBase request)
        {
            IEnumerable<string> targets = request.Form.GetValues("targets");
            NameValueCollection parameters = request.QueryString.Count > 0 ? request.QueryString : request.Form;
            if (targets == null)
            {
                string t = parameters["targets[]"];
                if (string.IsNullOrEmpty(t))
                    t = parameters["targets"];
                if (string.IsNullOrEmpty(t))
                    return null;
                targets = t.Split(',');
            }
            return targets;
        }
    }
}