using System.IO;

namespace ElFinder
{
    using System;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Collections.Generic;

    public static class StringExtension
    {        
        private const string InvalidFileNameChars = "[ .,_/\\\\+:;?!@]|<>'()^`";       

        public static string Transliterate(this string text, bool allowunicodeCharacters = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            var rgx = new Regex(InvalidFileNameChars);
            text = rgx.Replace(text, "-").Trim();
            return Utf8ToAsciiConverter.ToAsciiString(text).ToLower();            
        }                     
    }
}