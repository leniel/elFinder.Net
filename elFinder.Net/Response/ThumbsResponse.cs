using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class ThumbsResponse
    {
        private Dictionary<string, string> _images;

        [DataMember(Name = "images")]
        public Dictionary<string, string> Images { get { return _images; } }

        public ThumbsResponse()
        {
            _images = new Dictionary<string, string>();
        }
    }
}