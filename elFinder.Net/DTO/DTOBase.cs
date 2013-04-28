using System;
using System.IO;
using System.Runtime.Serialization;

namespace ElFinder.DTO
{
    [DataContract]
    internal abstract class DTOBase
    {
        protected static DateTime _unixOrigin = new DateTime(1970, 1, 1, 0, 0, 0);
        
        /// <summary>
        ///  Name of file/dir. Required
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; protected set; }
        
        /// <summary>
        ///  Hash of current file/dir path, first symbol must be letter, symbols before _underline_ - volume id, Required.
        /// </summary>
        [DataMember(Name = "hash")]
        public string Hash { get; protected set; } 
       
        /// <summary>
        ///  mime type. Required.
        /// </summary>
        [DataMember(Name = "mime")]
        public string Mime { get; protected set; } 
 
        /// <summary>
        /// file modification time in unix timestamp. Required.
        /// </summary>
        [DataMember(Name = "ts")]
        public long UnixTimeStamp { get; protected set; } 

        /// <summary>
        ///  file size in bytes
        /// </summary>
        [DataMember(Name = "size")]
        public long Size { get; protected set; } 

        /// <summary>
        ///  is readable
        /// </summary>
        [DataMember(Name = "read")]
        public byte Read { get; protected set; }

        /// <summary>
        /// is writable
        /// </summary>
        [DataMember(Name = "write")]
        public byte Write { get; protected set; }

        /// <summary>
        ///  is file locked. If locked that object cannot be deleted and renamed
        /// </summary>
        [DataMember(Name = "locked")]
        public byte Locked { get; protected set; }

        public static DTOBase Create(FileInfo info, Root root)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            if (root == null)
                throw new ArgumentNullException("root");
            string ext = info.Extension.ToLower();
            string parentPath = info.Directory.FullName.Substring(root.Directory.FullName.Length);
            FileDTO response;
            string hash = root.VolumeId + Helper.EncodePath(info.FullName.Substring(root.Directory.FullName.Length));
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || info.Extension == ".gif")
            {
                response = new ImageDTO();
                //((ImageDTO)response).Thumbnail = 
            }
            else
            {
                response = new FileDTO();
            }
            response.Read = 1;
            response.Write = root.IsReadOnly ? (byte)0 : (byte)1;
            response.Locked = root.IsReadOnly ? (byte)1 : (byte)0;
            response.Name = info.Name;
            response.Size = info.Length;
            response.UnixTimeStamp = (long)(info.LastWriteTimeUtc - _unixOrigin).TotalSeconds;
            response.Mime = Helper.GetMimeType(info);
            response.Hash = hash;
            response.ParentHash = root.VolumeId + Helper.EncodePath(parentPath.Length > 0 ? parentPath : info.Directory.Name);
            return response;
        }
        public static DTOBase Create(DirectoryInfo directory, Root root)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");
            if (root == null)
                throw new ArgumentNullException("root");
            if (root.Directory.FullName == directory.FullName)
            {
                RootDTO response = new RootDTO()
                {
                    Mime = "directory",
                    Dirs = directory.GetDirectories().Length > 0 ? (byte)1 : (byte)0,
                    Hash = root.VolumeId + Helper.EncodePath(directory.Name),
                    Locked = root.IsReadOnly ? (byte)1 : (byte)0,//(directory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ? (byte)1 : (byte)0,
                    Name = root.Alias,
                    Read = 1,
                    Size = 0,
                    UnixTimeStamp = (long)(directory.LastWriteTimeUtc - _unixOrigin).TotalSeconds,
                    VolumeId = root.VolumeId,
                    Write = root.IsReadOnly ? (byte)0 : (byte)1//(directory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ? (byte)0 : (byte)1
                };
                return response;
            }
            else
            {
                string parentPath = directory.Parent.FullName.Substring(root.Directory.FullName.Length);
                DirectoryDTO response = new DirectoryDTO()
                {
                    Mime = "directory",
                    ContainsChildDirs = directory.GetDirectories().Length > 0 ? (byte)1 : (byte)0,
                    Hash = root.VolumeId + Helper.EncodePath(directory.FullName.Substring(root.Directory.FullName.Length)),
                    Locked = root.IsReadOnly ? (byte)1 : (byte)0,//(directory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ? (byte)1 : (byte)0,
                    Read = 1,
                    Size = 0,
                    Name = directory.Name,
                    UnixTimeStamp = (long)(directory.LastWriteTimeUtc - _unixOrigin).TotalSeconds,
                    Write = root.IsReadOnly ? (byte)0 : (byte)1,//(directory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ? (byte)0: (byte)1,
                    ParentHash = root.VolumeId + Helper.EncodePath(parentPath.Length > 0 ? parentPath : directory.Parent.Name)
                };
                return response;
            }
        }
       
    }
}