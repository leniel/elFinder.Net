using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class Debug
    {
        private static string[] _empty = new string[0];
        [DataMember(Name = "connector")]
        public string Connector { get { return ".net"; } }

        [DataMember(Name = "mountErrors")]
        public string[] MountErrors { get { return _empty; } }
    }
}
