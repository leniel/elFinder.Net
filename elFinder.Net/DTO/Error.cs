using System.Web.Mvc;

namespace ElFinder.DTO
{
    internal static class Error
    {       
        public static JsonResult CommandNotFound()
        {
            return FormatSimpleError("errUnknownCmd");
        }
        public static JsonResult MissedParameter(string command)
        {
            return Json(new { error = new string[] { "errCmdParams", command } });
        }
        public static JsonResult CannotUploadFile()
        {
            return FormatSimpleError("errUploadFile");
        }
        public static JsonResult MaxUploadFileSize()
        {
            return FormatSimpleError("errFileMaxSize");
        }
        public static JsonResult AccessDenied()
        {
            return FormatSimpleError("errAccess");
        }
        private static JsonResult FormatSimpleError(string message)
        {
            return Json(new { error = message });
        }
        private static JsonResult Json(object data)
        {
            return new JsonDataContractResult(data) { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}