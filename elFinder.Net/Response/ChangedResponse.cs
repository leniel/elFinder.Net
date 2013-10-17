using System.Collections.Generic;
using System.Runtime.Serialization;
using ElFinder.DTO;

namespace ElFinder.Response
{
    [DataContract]
    internal class ChangedResponse
    {
        [DataMember(Name="changed")]
        public List<FileDTO> Changed { get; private set; }

        public ChangedResponse()
        {
            Changed = new List<FileDTO>();
        }
    }
}