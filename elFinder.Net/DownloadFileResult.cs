using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ElFinder
{
    internal class DownloadFileResult : ActionResult
    {
        public FileInfo File { get; private set; }
        public bool IsDownload { get; private set; }
        public DownloadFileResult(FileInfo file, bool isDownload)
        {
            File = file;
            IsDownload = isDownload;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            HttpRequestBase request = context.HttpContext.Request;
            if (!HttpCacheHelper.IsFileFromCache(File, request, response))
            {

                string fileName;
                string fileNameEncoded = HttpUtility.UrlEncode(File.Name);

                if (context.HttpContext.Request.UserAgent.Contains("MSIE")) // IE < 9 do not support RFC 6266 (RFC 2231/RFC 5987)
                {
                    fileName = "filename=\"" + fileNameEncoded + "\"";
                }
                else
                {
                    fileName = "filename*=UTF-8\'\'" + fileNameEncoded; // RFC 6266 (RFC 2231/RFC 5987)
                }
                string mime;
                string disposition;
                if (IsDownload)
                {
                    mime = "application/octet-stream";
                    disposition = "attachment; " + fileName;
                }
                else
                {
                    mime = Helper.GetMimeType(File);
                    disposition = (mime.Contains("image") || mime.Contains("text") || mime == "application/x-shockwave-flash" ? "inline; " : "attachment; ") + fileName;
                }

                response.ContentType = mime;
                response.AppendHeader("Content-Disposition", disposition);
                response.AppendHeader("Content-Location", File.Name);
                response.AppendHeader("Content-Transfer-Encoding", "binary");
                response.AppendHeader("Content-Length", File.Length.ToString());
                response.WriteFile(File.FullName);
                response.End();
                response.Flush();
            }
            else
            {
                response.ContentType = IsDownload ? "application/octet-stream" : Helper.GetMimeType(File);
                response.End();
            }
        }
    }
}
