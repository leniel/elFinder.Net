using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class GetResponse
    {
        [DataMember(Name="content")]
        public string Content { get; set; }
    }
}