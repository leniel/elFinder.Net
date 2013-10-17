using System.Drawing;
using System.Runtime.Serialization;

namespace ElFinder.Response
{
    [DataContract]
    internal class DimResponse
    {
        [DataMember(Name="dim")]
        public string Dimension { get; set; }
        public DimResponse(string dimension)
        {
            Dimension = dimension;
        }
        public DimResponse(Size size)
        {
            Dimension = string.Format("{0}x{1}", size.Width, size.Height);
        }
    }
}