using ElFinder.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class TreeResponse
    {
        [DataMember(Name="tree")]
        public List<DTOBase> Tree { get; private set; }

        public TreeResponse()
        {
            Tree = new List<DTOBase>();
        }     
    }
}