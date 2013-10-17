using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ElFinder.DTO;

namespace ElFinder.Response
{
    [DataContract]
    internal class AddResponse
    {
        private List<DTOBase> _added;

        [DataMember(Name = "added")]
        public List<DTOBase> Added { get { return _added; } }

        public AddResponse(FileInfo newFile, Root root)
        {
            _added = new List<DTOBase>() { DTOBase.Create(newFile, root) };
        }
        public AddResponse(DirectoryInfo newDir, Root root)
        {
            _added = new List<DTOBase>() { DTOBase.Create(newDir, root) };
        }
        public AddResponse()
        {
            _added = new List<DTOBase>();
        }
    }
}