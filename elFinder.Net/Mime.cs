using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace ElFinder
{
    using System;

    internal static class Mime
    {
        private static Dictionary<string, string> _mimeTypes;
        
        static Mime()
        {
            _mimeTypes = new Dictionary<string, string>();
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream("elFinder.Net.mimeTypes.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        {
                            continue;
                        }

                        var parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length > 1)
                        {
                            var mime = parts[0];

                            for (var i = 1; i < parts.Length; i++)
                            {
                                var ext = parts[i].ToLower();
                                if (!_mimeTypes.ContainsKey(ext))
                                {
                                    _mimeTypes.Add(ext, mime);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string GetMimeType(string extension)
        {
            if (_mimeTypes.ContainsKey(extension))
            {
                return _mimeTypes[extension];
            }
            
            return "unknown";
        }
    }
}
