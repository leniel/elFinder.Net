using System.Collections.Generic;
using System.Runtime.Serialization;
using ElFinder.DTO;

namespace ElFinder.Response
{
    [DataContract]
    internal class ListResponse
    {
        [DataMember(Name="list")]
        public List<string> List { get; private set; }

        public ListResponse()
        {
            List = new List<string>();
        }     
    }
}