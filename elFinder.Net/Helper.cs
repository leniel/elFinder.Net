using System.IO;
using System.Web;

namespace ElFinder
{
    internal static class Helper
    {
        public static string GetMimeType(FileInfo file)
        {
            return Mime.GetMimeType(file.Extension.ToLower().Substring(1));
        }

        public static string GetMimeType(string ext)
        {
            return Mime.GetMimeType(ext);
        }
        public static string EncodePath(string path)
        {
            return HttpServerUtility.UrlTokenEncode(System.Text.UTF8Encoding.UTF8.GetBytes(path));
        }
        public static string DecodePath(string path)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(path));
        }

        public static string Duplicate(FileInfo file)
        {
            var parentPath = file.DirectoryName;

            var name = file.Name;

            var ext = string.Empty;

            var nameArr = name.Split(".".ToCharArray());

            if (nameArr.Length > 1)
            {

                ext = "." + nameArr[nameArr.Length - 1];
                name = name.Remove(name.LastIndexOf("."));
            }

            var newName = string.Format(@"{0}\{1} copy{2}", parentPath, name, ext);

            if (!File.Exists(newName))
            {
                file.CopyTo(newName);
            }
            else
            {
                for (int i = 1; i < 100; i++)
                {
                    newName = string.Format(@"{0}\{1} copy {2}{3}", parentPath, name, i, ext);
                    if (!File.Exists(newName))
                    {
                        file.CopyTo(newName);
                        break;
                    }
                }
            }

            return newName;
        }
    }
}