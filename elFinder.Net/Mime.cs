using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace ElFinder
{
    internal static class Mime
    {
        private static Dictionary<string, string> _mimeTypes;
        static Mime()
        {
            _mimeTypes = new Dictionary<string, string>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            if(assembly != null)
            {
                using (Stream stream = assembly.GetManifestResourceStream("ElFinder.mimeTypes.txt"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            line = line.Trim();
                            if (!string.IsNullOrWhiteSpace(line) && line[0] != '#')
                            {
                                string[] parts = line.Split(' ');
                                if (parts.Length > 1)
                                {
                                    string mime = parts[0];
                                    for (int i = 1; i < parts.Length; i++)
                                    {
                                        string ext = parts[i].ToLower();
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
            }
        }
        public static string GetMimeType(string extension)
        {
            if (_mimeTypes.ContainsKey(extension))
            {
                return _mimeTypes[extension];
            }
            else
            {
                return "unknown";
            }                    
        }
    }
}
