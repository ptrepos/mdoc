using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mdoc.Parsers;

namespace Mdoc
{
    public class Mdoc
    {
        public static Section[] Parse(TextReader reader)
        {
            SectionParser parser = new SectionParser(reader);
            return parser.Parse();
        }

        public static Section[] Parse(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return Parse(reader);
            }
        }

        public static Section[] Parse(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                return Parse(reader);
            }
        }
    }
}
