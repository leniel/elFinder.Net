using System.Runtime.Serialization;

namespace ElFinder.DTO
{
    [DataContract]
    internal class ImageDTO : FileDTO
    {   
        [DataMember(Name = "tmb")]
        public string Thumbnail { get; set; }
    }
}