using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mdoc;
using Mdoc.Encoders;

namespace MdocConv
{
    class Program
    {
        static void Main(string[] args)
        {
            string mdocPath = args[0];
            string outputPath = Path.Combine(
                    Path.GetDirectoryName(mdocPath),
                    Path.GetFileNameWithoutExtension(mdocPath) + ".html");

            Section[] sections = Mdoc.Mdoc.Parse(mdocPath);

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<meta charset=\"UTF-8\">");
                writer.WriteLine("<title>{0}</title>", Path.GetFileNameWithoutExtension(mdocPath));
                writer.WriteLine("<link href=\"style/doc.css\" rel=\"stylesheet\" type =\"text/css\" />");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");

                HtmlEncoder encoder = new HtmlEncoder();
                encoder.Encode(writer, sections);

                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }
    }
}
