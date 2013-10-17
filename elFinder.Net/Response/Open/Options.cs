using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ElFinder.Response
{
    [DataContract]
    internal class Archive
    {
        private static string[] _empty = new string[0];
        [DataMember(Name = "create")]
        public IEnumerable<string> Create { get { return _empty; } }

        [DataMember(Name = "extract")]
        public IEnumerable<string> Extract { get { return _empty; } }
    }
    [DataContract]
    internal class Options
    {
        private static string[] _empty = new string[0];
        private static string[] _disabled = new string[] { "extract", "create" };
        private static Archive _emptyArchives = new Archive();

        [DataMember(Name = "copyOverwrite")]
        public byte IsCopyOverwrite { get { return 1; } }        

        [DataMember(Name = "separator")]
        public char Separator { get { return '/'; } }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "tmbUrl")]
        public string ThumbnailsUrl { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "archivers")]
        public Archive Archivers { get { return _emptyArchives; } }

        [DataMember(Name = "disabled")]
        public IEnumerable<string> Disabled { get { return _disabled; } }

        public Options(FullPath fullPath)
        {

            Path = fullPath.Root.Alias;
            if (fullPath.RelativePath != string.Empty)
                Path += Separator + fullPath.RelativePath.Replace('\\', Separator);
            Url = fullPath.Root.Url ?? string.Empty;
            ThumbnailsUrl = fullPath.Root.ThumbnailsUrl ?? string.Empty;
        }
    }
}