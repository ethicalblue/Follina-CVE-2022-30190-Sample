using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Follina_PoC
{
    internal class FollinaExtractor
    {
        public List<string> ReadFollinaInfectedDocument(string path)
        {
            List<string> hits = new();

            if (string.IsNullOrEmpty(path) == false)
            {
                if (Path.GetExtension(path).ToLower() == ".docx")
                {
                    using MemoryStream memoryStream = new();
                    using ZipArchive archive = ZipFile.OpenRead(path);
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName == "word/_rels/document.xml.rels")
                        {
                            using var entryStream = entry.Open();
                            using var streamReader = new StreamReader(entryStream);
                            var text = streamReader.ReadToEnd();

                            int start = text.IndexOf("Target=\"mhtml:");
                            
                            const string prefix = "Target=\"";

                            if (start != -1)
                            {
                                hits.Add(text[(start + prefix.Length)..text.IndexOf("\"", start + prefix.Length)]);
                            }
                        }
                    }
                }
                else if (Path.GetExtension(path).ToLower() == ".rtf")
                {
                    string text = File.ReadAllText(path);

                    int start = text.IndexOf("\\objclass http");

                    const string prefix1 = "\\objclass ";
                    const string prefix2 = "\\objclass (";

                    if (start != -1)
                    {
                        hits.Add(text[(start + prefix1.Length)..text.IndexOf("}", start)]);
                    }

                    start = text.IndexOf("\\objclass (http");

                    if(start != -1)
                    {
                        hits.Add(text[(start + prefix2.Length)..text.IndexOf("}", start)]);
                    }
                }
            }

            return hits;
        }
    }
}
