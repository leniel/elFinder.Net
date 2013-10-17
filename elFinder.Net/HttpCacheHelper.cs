using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ElFinder
{
    internal static class HttpCacheHelper
    {
        public static bool IsFileFromCache(FileInfo info, HttpRequestBase request, HttpResponseBase response)
        {
            DateTime updated = info.LastWriteTimeUtc;
            string filename = info.Name;
            DateTime modifyDate;
            if (!DateTime.TryParse(request.Headers["If-Modified-Since"], out modifyDate))
            {
                modifyDate = DateTime.UtcNow;
            }
            string eTag = GetFileETag(filename, updated);            
            if (!IsFileModified(updated, eTag, request))
            {
                response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
                response.StatusDescription = "Not Modified";
                response.AddHeader("Content-Length", "0");
                response.Cache.SetCacheability(HttpCacheability.Public);
                response.Cache.SetLastModified(updated);
                response.Cache.SetETag(eTag);
                return true;
            }
            else
            {
                response.Cache.SetAllowResponseInBrowserHistory(true);
                response.Cache.SetCacheability(HttpCacheability.Public);
                response.Cache.SetLastModified(updated);
                response.Cache.SetETag(eTag);
                return false;
            }
        }

        private static string GetFileETag(string fileName, DateTime modified)
        {
            return "\"" + Helper.GetFileMd5(fileName, modified) + "\"";
        }

        private static bool IsFileModified(DateTime modifyDate, string eTag, HttpRequestBase request)
        {
            DateTime modifiedSince;
            bool fileDateModified = true;

            //Check If-Modified-Since request header, if it exists 
            if (!string.IsNullOrEmpty(request.Headers["If-Modified-Since"]) && DateTime.TryParse(request.Headers["If-Modified-Since"], out modifiedSince))
            {
                fileDateModified = false;
                if (modifyDate > modifiedSince)
                {
                    TimeSpan modifyDiff = modifyDate - modifiedSince;
                    //ignore time difference of up to one seconds to compensate for date encoding
                    fileDateModified = modifyDiff > TimeSpan.FromSeconds(1);
                }
            }

            //check the If-None-Match header, if it exists, this header is used by FireFox to validate entities based on the etag response header 
            bool eTagChanged = false;
            if (!string.IsNullOrEmpty(request.Headers["If-None-Match"]))
            {
                eTagChanged = request.Headers["If-None-Match"] != eTag;
            }
            return (eTagChanged || fileDateModified);
        }

       
    }
}
