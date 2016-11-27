using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class Debug
    {
        [DataMember(Name = "connector")]
        public string Connector { get { return ".net"; } }
    }
}