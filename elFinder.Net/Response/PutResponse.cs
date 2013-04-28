using ElFinder.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class PutResponse
    {
        [DataMember(Name="changed")]
        public List<FileDTO> Changed { get; private set; }

        public PutResponse()
        {
            Changed = new List<FileDTO>();
        }
    }
}