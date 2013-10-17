using System.IO;
using System;

namespace ElFinder
{
    public class FullPath
    {
        public Root Root
        {
            get { return _root; }
        }
        public bool IsDirectoty
        {
            get { return _isDirectory; }
        }
        public string RelativePath
        {
            get
            {
                return _relativePath;
            }
        }
        public DirectoryInfo Directory
        {
            get
            {
                return _isDirectory ? (DirectoryInfo)_fileSystemObject : null;
            }
        }
        public FileInfo File
        {
            get
            {
                return !_isDirectory ? (FileInfo)_fileSystemObject : null;
            }
        }
        public FullPath(Root root, FileSystemInfo fileSystemObject)
        {
            if (root == null)
                throw new ArgumentNullException("root", "Root can not be null");
            if (fileSystemObject == null)
                throw new ArgumentNullException("root", "Filesystem object can not be null");
            _root = root;
            _fileSystemObject = fileSystemObject;
            _isDirectory = _fileSystemObject is DirectoryInfo;
            if (fileSystemObject.FullName.StartsWith(root.Directory.FullName))
            {
                if (fileSystemObject.FullName.Length == root.Directory.FullName.Length)
                {
                    _relativePath = string.Empty;
                }
                else
                {
                    _relativePath = fileSystemObject.FullName.Substring(root.Directory.FullName.Length + 1);
                }
            }
            else
                throw new InvalidOperationException("Filesystem object must be in it root directory or in root subdirectory");

        }


        private Root _root;
        private FileSystemInfo _fileSystemObject;
        private bool _isDirectory;
        private string _relativePath;

        
        

    }
}