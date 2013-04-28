using ElFinder.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class InitResponse : OpenResponseBase
    {
        private static string[] _empty = new string[0];
        [DataMember(Name="api")]
        public string Api { get { return "2.0"; } }

        [DataMember(Name = "uplMaxSize")]
        public string UploadMaxSize { get { return string.Empty; } }

        [DataMember(Name = "netDrivers")]
        public IEnumerable<string> NetDrivers { get { return _empty; } }

        public InitResponse(DTOBase currentWorkingDirectory)
            : base(currentWorkingDirectory)
        {
            Options = new Options();
        }
    }
}