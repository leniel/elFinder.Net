using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ElFinder.DTO
{
    [DataContract]
    internal class FileDTO : DTOBase
    {          
        /// <summary>
        ///  Hash of parent directory. Required except roots dirs.
        /// </summary>
        [DataMember(Name = "phash")]
        public string ParentHash { get; set; }
    }
}