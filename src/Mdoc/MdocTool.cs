using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mdoc.Parsers;
using Mdoc.Encoders;

namespace Mdoc
{
    public class MdocTool
    {
        public const string FILE_EXTENSION = ".mdoc";

        public static Section[] Parse(TextReader reader, TextWriter messageWriter)
        {
            SectionParser parser = new SectionParser(reader);
            parser.MessageWriter = messageWriter;
            return parser.Parse();
        }

        public static Section[] Parse(Stream stream, TextWriter messageWriter)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return Parse(reader, messageWriter);
            }
        }

        public static Section[] Parse(string filePath, TextWriter messageWriter)
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                return Parse(reader, messageWriter);
            }
        }

        public static void EncodeHtml(
                        TextWriter writer,
                        TextReader reader,
                        TextWriter messageWriter,
                        HtmlEncodeParameters parameters)
        {
            Section[] sections = Mdoc.MdocTool.Parse(reader, messageWriter);

            if (parameters.HeaderIncludes)
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<meta charset=\"UTF-8\">");

                if (!string.IsNullOrEmpty(parameters.DocumentTitle))
                    writer.WriteLine("<title>{0}</title>", parameters.DocumentTitle);
                if (parameters.CssUrl != null)
                    writer.WriteLine("<link href=\"{0}\" rel=\"stylesheet\" type =\"text/css\" />", parameters.CssUrl.ToString());

                writer.WriteLine("</head>");
                writer.WriteLine("<body>");
            }

            HtmlEncoder encoder = new HtmlEncoder();
            encoder.Encode(writer, sections);

            if (parameters.HeaderIncludes)
            {
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }
    }
}
