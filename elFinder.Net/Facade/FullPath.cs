using System.IO;

namespace ElFinder
{
    public class FullPath
    {
        public Root Root { get; set; }
        public string RelativePath { get; set; }
        public DirectoryInfo Directory { get; set; }
        public FileInfo File { get; set; }
    }
}