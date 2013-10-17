using System.Collections.Generic;
using System.Runtime.Serialization;
using ElFinder.DTO;

namespace ElFinder.Response
{
    [DataContract]
    internal class OpenResponseBase
    {
        [DataMember(Name="files")]
        public List<DTOBase> Files { get { return _files; } }

        [DataMember(Name = "cwd")]
        public DTOBase CurrentWorkingDirectory { get { return _currentWorkingDirectory; } }

        [DataMember(Name = "options")]
        public Options Options { get; protected set; }

        [DataMember(Name = "debug")]
        public Debug Debug { get { return _debug; } }

        public OpenResponseBase(DTOBase currentWorkingDirectory)
        {
            _files = new List<DTOBase>();
            _currentWorkingDirectory = currentWorkingDirectory;
        }

        private static Debug _debug = new Debug();
        protected List<DTOBase> _files;
        private DTOBase _currentWorkingDirectory;
    }
}