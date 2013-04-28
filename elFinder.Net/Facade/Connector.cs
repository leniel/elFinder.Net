using ElFinder.DTO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;

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
                        IEnumerable<string> targets = request.Form.GetValues("targets");
                        if (targets == null)
                        {
                            var t = parameters["targets[]"];
                            if (string.IsNullOrEmpty(t))
                                t = parameters["targets"];
                            if (string.IsNullOrEmpty(t))
                                return Error.MissedParameter("targets");
                            targets = t.Split(',');
                        }
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
                        IEnumerable<string> targets = request.Form.GetValues("targets");
                        if (targets == null)
                        {
                            var t = parameters["targets[]"];
                            if (string.IsNullOrEmpty(t))
                                t = parameters["targets"];
                            if (string.IsNullOrEmpty(t))
                                return Error.MissedParameter("targets");
                            targets = t.Split(',');
                        }
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
                        IEnumerable<string> targets = request.Form.GetValues("targets");
                        if (targets == null)
                        {
                            var t = parameters["targets[]"];
                            if (string.IsNullOrEmpty(t))
                                t = parameters["targets"];
                            if (string.IsNullOrEmpty(t))
                                return Error.MissedParameter("targets");
                            targets = t.Split(',');
                        }
                        return _driver.Duplicate(targets);
                    }
                default:
                    return Error.CommandNotFound();
            }
        }

        public FileSystemInfo GetFileByHash(string hash)
        {
            FullPath path = _driver.ParsePath(hash);
            return path.Directory == null ? (FileSystemInfo)path.File : (FileSystemInfo)path.Directory;
        }
    }
}