using ElFinder.DTO;
using ElFinder.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ElFinder
{
    /// <summary>
    /// Represents a driver for local file system
    /// </summary>
    public class FileSystemDriver : IDriver
    {
        #region private  
        private const string _volumePrefix = "v";
        private List<Root> _roots;
        
        private JsonResult Json(object data)
        {
            return new JsonDataContractResult(data) { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        private void DirectoryCopy(DirectoryInfo sourceDir, string destDirName, bool copySubDirs)
        { 
            DirectoryInfo[] dirs = sourceDir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDir.FullName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = sourceDir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir, temppath, copySubDirs);
                }
            }
        }

        #endregion

        #region public 
        
        public FullPath ParsePath(string target)
        {
            StringBuilder volumeIdBuilder = new StringBuilder();
            StringBuilder pathBuilder = null;
            foreach (var c in target)
            {
                if (pathBuilder != null)
                {
                    pathBuilder.Append(c);
                }
                else
                {
                    volumeIdBuilder.Append(c);
                    if (c == '_')
                    {
                        pathBuilder = new StringBuilder();
                    }
                }
            }
            Root root = _roots.First(r => r.VolumeId == volumeIdBuilder.ToString());
            string path = Helper.DecodePath(pathBuilder.ToString());
            string dirUrl = path != root.Directory.Name ? path : string.Empty;
            var dir = new DirectoryInfo(root.Directory.FullName + dirUrl);
            if (dir.Exists)
            {
                string parentPath = dir.FullName.Substring(root.Directory.FullName.Length).Replace('\\', '/');
                return new FullPath() { Directory = dir, Root = root, RelativePath = root.Alias + parentPath };
            }
            else
            {
                var file = new FileInfo(root.Directory.FullName + dirUrl);
                string parentPath = file.FullName.Substring(root.Directory.FullName.Length).Replace('\\', '/');
                return new FullPath() { File = file, Root = root, RelativePath = root.Alias + parentPath };
            }
        }

        /// <summary>
        /// Initialize new instance of class ElFinder.FileSystemDriver 
        /// </summary>
        public FileSystemDriver()
        {
            _roots = new List<Root>();
        }

        /// <summary>
        /// Adds an object to the end of the roots.
        /// </summary>
        /// <param name="item"></param>
        public void AddRoot(Root item)
        {
            _roots.Add(item);
            item.VolumeId = _volumePrefix + _roots.Count + "_";
        }

        /// <summary>
        /// Gets collection of roots
        /// </summary>
        public IEnumerable<Root> Roots { get { return _roots; } }
        #endregion public

        #region   IDriver
        JsonResult IDriver.Open(string target, bool tree)
        {
            FullPath fullPath = ParsePath(target);
            OpenResponse answer = new OpenResponse(DTOBase.Create(fullPath.Directory, fullPath.Root), fullPath);
            foreach (var item in fullPath.Directory.GetFiles())
            {
                answer.AddResponse(DTOBase.Create(item, fullPath.Root));
            }
            foreach (var item in fullPath.Directory.GetDirectories())
            {
                answer.AddResponse(DTOBase.Create(item, fullPath.Root));
            }
            return Json(answer);
        }
        JsonResult IDriver.Init(string target)
        {
            Root root;
            DirectoryInfo dir;
            if (string.IsNullOrEmpty(target))
            {
                root = _roots.FirstOrDefault(r => r.StartPath != null);
                if (root == null)
                    root = _roots.First();
                dir = root.StartPath == null ? root.Directory : root.StartPath;
            }
            else
            {
                FullPath fullPath = ParsePath(target);
                root = fullPath.Root;
                dir = fullPath.Directory;
            }
            InitResponse answer = new InitResponse(DTOBase.Create(dir, root));
            

            foreach (var item in dir.GetFiles())
            {
                answer.AddResponse(DTOBase.Create(item, root));
            }
            foreach (var item in dir.GetDirectories())
            {
                answer.AddResponse(DTOBase.Create(item, root));
            }
            foreach (var item in _roots)
            {
                answer.AddResponse(DTOBase.Create(item.Directory, item));
            }
            if (root.Directory.FullName != dir.FullName)
            {
                foreach (var item in root.Directory.GetDirectories())
                {
                    answer.AddResponse(DTOBase.Create(item, root));
                }
            }
            string parentPath = string.IsNullOrEmpty(target) ? root.Alias : root.Alias + dir.FullName.Substring(root.Directory.FullName.Length).Replace('\\', '/');
            answer.Options.Path = parentPath;
            answer.Options.Url = root.Url;
            answer.Options.ThumbnailsUrl = root.TmbUrl;
            return Json(answer);
        }
        ActionResult IDriver.File(string target, bool download)
        {
            FullPath fullPath = ParsePath(target);
            if (!fullPath.File.Exists)
                return new HttpNotFoundResult("File not found");
            if (fullPath.Root.IsShowOnly)
                return new HttpStatusCodeResult(403, "Access denied");
            return new DownloadFileResult(fullPath.File, download);
        }
        JsonResult IDriver.Parents(string target)
        {
            FullPath fullPath = ParsePath(target);
            TreeResponse answer = new TreeResponse();
            if (fullPath.Directory.FullName == fullPath.Root.Directory.FullName)
            {
                answer.Tree.Add(DTOBase.Create(fullPath.Directory, fullPath.Root)); 
            }
            else
            {
                DirectoryInfo parent = fullPath.Directory;
                foreach (var item in parent.Parent.GetDirectories())
                {
                    answer.Tree.Add(DTOBase.Create(item, fullPath.Root));
                }
                while (parent.FullName != fullPath.Root.Directory.FullName)
                {
                    parent = parent.Parent;
                    answer.Tree.Add(DTOBase.Create(parent, fullPath.Root));
                }
            }
            return Json(answer);
        }
        JsonResult IDriver.Tree(string target)
        {
            FullPath fullPath = ParsePath(target);
            TreeResponse answer = new TreeResponse();
            foreach (var item in fullPath.Directory.GetDirectories())
            {
                answer.Tree.Add(DTOBase.Create(item, fullPath.Root));
            }
            return Json(answer);
        }
        JsonResult IDriver.List(string target)
        {
            FullPath fullPath = ParsePath(target);
            ListResponse answer = new ListResponse();
            foreach (var item in fullPath.Directory.GetFileSystemInfos())
            {
                answer.List.Add(item.Name);
            }
            return Json(answer);
        }
        JsonResult IDriver.MakeDir(string target, string name)
        {
            FullPath fullPath = ParsePath(target);
            DirectoryInfo newDir = Directory.CreateDirectory(Path.Combine(fullPath.Directory.FullName, name));
            return Json(new AddResponse(newDir, fullPath.Root));
        }
        JsonResult IDriver.MakeFile(string target, string name)
        {
            FullPath fullPath = ParsePath(target);
            FileInfo newFile = new FileInfo(Path.Combine(fullPath.Directory.FullName, name));
            newFile.Create().Close();            
            return Json(new AddResponse(newFile, fullPath.Root));
        }
        JsonResult IDriver.Rename(string target, string name)
        {
            FullPath fullPath = ParsePath(target);
            var answer = new ReplaceResponse();
            answer.Removed.Add(target);
            if (fullPath.Directory != null)
            { 
                fullPath.Directory.MoveTo(Path.Combine(fullPath.Directory.Parent.FullName, name));
                var newDir = new DirectoryInfo(Path.Combine(fullPath.Directory.Parent.FullName, name));
                answer.Added.Add(DTOBase.Create(newDir, fullPath.Root));

            }
            else
            {
                fullPath.File.MoveTo(Path.Combine(fullPath.File.Directory.FullName, name));
                var newFile = new FileInfo(Path.Combine(fullPath.File.Directory.FullName, name));
                answer.Added.Add(DTOBase.Create(newFile, fullPath.Root));
            }
            return Json(answer);
        }
        JsonResult IDriver.Remove(IEnumerable<string> targets)
        {
            RemoveResponse answer = new RemoveResponse();
            foreach (var item in targets)
            {
                FullPath fullPath = ParsePath(item);
                if (fullPath.Directory != null)
                {
                    fullPath.Directory.Delete(true);
                }
                else
                {
                    fullPath.File.Delete();
                }
                answer.Removed.Add(item);
            }
            return Json(answer);
        }
        JsonResult IDriver.Get(string target)
        {
            FullPath fullPath = ParsePath(target);
            GetResponse answer =  new GetResponse();
            using (StreamReader reader = new StreamReader(fullPath.File.OpenRead()))
            {
                answer.Content = reader.ReadToEnd();
            }
            return Json(answer);
        }
        JsonResult IDriver.Put(string target, string content)
        {
            FullPath fullPath = ParsePath(target);
            PutResponse answer = new PutResponse();
            using (StreamWriter writer = new StreamWriter(fullPath.File.FullName, false))
            {
                writer.Write(content);
            }
            answer.Changed.Add((FileDTO)DTOBase.Create(fullPath.File, fullPath.Root));
            return Json(answer);
        }
        JsonResult IDriver.Paste(string source, string dest, IEnumerable<string> targets, bool isCut)
        {
            FullPath destPath = ParsePath(dest);
            ReplaceResponse response = new ReplaceResponse();
            foreach (var item in targets)
            {
                FullPath src = ParsePath(item);
                if (src.Directory != null)
                {
                    DirectoryInfo newDir = new DirectoryInfo(Path.Combine(destPath.Directory.FullName, src.Directory.Name));
                    if (newDir.Exists)
                        Directory.Delete(newDir.FullName, true);
                    if (isCut)
                    {
                        src.Directory.MoveTo(newDir.FullName);
                        response.Removed.Add(item);
                    }
                    else
                    {
                        DirectoryCopy(src.Directory, newDir.FullName, true);
                    }
                    response.Added.Add(DTOBase.Create(newDir, destPath.Root));
                }
                else
                {
                    string newFilePath = Path.Combine(destPath.Directory.FullName, src.File.Name);
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                    if (isCut)
                    {
                        src.File.MoveTo(newFilePath);
                        response.Removed.Add(item);
                    }
                    else
                    {
                        File.Copy(src.File.FullName, newFilePath);
                    }
                    response.Added.Add(DTOBase.Create(new FileInfo(newFilePath), destPath.Root));
                }
            }
            return Json(response);
        }
        JsonResult IDriver.Upload(string target, System.Web.HttpFileCollectionBase targets)
        {
            FullPath dest = ParsePath(target);
            var response = new AddResponse();
            if (dest.Root.MaxUploadSize.HasValue)
            {
                for (int i = 0; i < targets.AllKeys.Length; i++)
                {
                    HttpPostedFileBase file = targets[i];
                    if (file.ContentLength > dest.Root.MaxUploadSize.Value)
                    {
                        return Error.MaxUploadFileSize();
                    }
                }
            }
            for (int i = 0; i < targets.AllKeys.Length; i++)
            {
                HttpPostedFileBase file = targets[i];                
                string path = Path.Combine(dest.Directory.FullName, file.FileName);

                if (File.Exists(path))
                {
                    if (dest.Root.UploadOverwrite)
                    {
                        //if file already exist we rename the current file, 
                        //and if upload is succesfully delete temp file, in otherwise we restore old file
                        string tmpPath = path + Guid.NewGuid();
                        bool uploaded = false;
                        try
                        {
                            File.Move(path, tmpPath);
                            file.SaveAs(path);
                            uploaded = true;
                        }
                        catch { }
                        finally
                        {
                            if (uploaded)
                            {
                                File.Delete(tmpPath);
                            }
                            else
                            {
                                if (File.Exists(path))
                                    File.Delete(path);
                                File.Move(tmpPath, path);
                            }
                        }
                    }
                    else
                    {
                        string name = null;
                        for (int j = 1; j < 100; j++)
                        {
                            string suggestName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + j + Path.GetExtension(file.FileName);
                            if (!File.Exists(Path.Combine(dest.Directory.FullName, suggestName)))
                            {
                                name = suggestName;
                                break;
                            }
                        }
                        if (name == null)
                            name = Path.GetFileNameWithoutExtension(file.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                        path = Path.Combine(dest.Directory.FullName, name);
                        file.SaveAs(path);
                    }
                }
                else
                {
                    file.SaveAs(path);
                }                
                response.Added.Add((FileDTO)DTOBase.Create(new FileInfo(path), dest.Root));
            }
            return Json(response);
        }
        JsonResult IDriver.Duplicate(IEnumerable<string> targets)
        {
            AddResponse response = new AddResponse();
            foreach (var target in targets)
            {
                FullPath fullPath = ParsePath(target);
                if (fullPath.Directory != null)
                {
                    var parentPath = fullPath.Directory.Parent.FullName;
                    var name = fullPath.Directory.Name;
                    var newName = string.Format(@"{0}\{1} copy", parentPath, name);
                    if (!Directory.Exists(newName))
                    {
                        DirectoryCopy(fullPath.Directory, newName, true);
                    }
                    else
                    {
                        for (int i = 1; i < 100; i++)
                        {
                            newName = string.Format(@"{0}\{1} copy {2}", parentPath, name, i);
                            if (!Directory.Exists(newName))
                            {
                                DirectoryCopy(fullPath.Directory, newName, true);
                                break;
                            }
                        }
                    }
                    response.Added.Add(DTOBase.Create(new DirectoryInfo(newName), fullPath.Root));
                }
                else
                {
                    var parentPath = fullPath.File.Directory.FullName;
                    var name = fullPath.File.Name.Substring(0, fullPath.File.Name.Length - fullPath.File.Extension.Length);
                    var ext = fullPath.File.Extension;

                    var newName = string.Format(@"{0}\{1} copy{2}", parentPath, name, ext);

                    if (!File.Exists(newName))
                    {
                        fullPath.File.CopyTo(newName);
                    }
                    else
                    {
                        for (int i = 1; i < 100; i++)
                        {
                            newName = string.Format(@"{0}\{1} copy {2}{3}", parentPath, name, i, ext);
                            if (!File.Exists(newName))
                            {
                                fullPath.File.CopyTo(newName);
                                break;
                            }
                        }
                    }
                    response.Added.Add(DTOBase.Create(new FileInfo(newName), fullPath.Root));
                }
            }
            return Json(response);
        }

        #endregion IDriver
    }
}