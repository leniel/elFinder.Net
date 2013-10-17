using System.Runtime.Serialization;
using ElFinder.DTO;

namespace ElFinder.Response
{
    [DataContract]
    internal class OpenResponse : OpenResponseBase
    {
        public OpenResponse(DTOBase currentWorkingDirectory, FullPath fullPath)
            : base(currentWorkingDirectory)
        {
            Options = new Options(fullPath);
            _files.Add(currentWorkingDirectory);
        }
    }
}