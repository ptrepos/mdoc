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
        public const string HTML_TYPE = "html";

        static void Main(string[] args)
        {
            if (ParseArgs(args) == false)
                return;

            string mdocPath = inputFile;
            string outputPath = Path.Combine(outputDir, outputFile);

            Section[] sections = Mdoc.MdocTool.Parse(mdocPath);

            if (type == HTML_TYPE)
            {
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("<!DOCTYPE html>");
                    writer.WriteLine("<html>");
                    writer.WriteLine("<head>");
                    writer.WriteLine("<meta charset=\"UTF-8\">");
                    writer.WriteLine("<title>{0}</title>", Path.GetFileNameWithoutExtension(mdocPath));
                    writer.WriteLine("<link href=\"{0}\" rel=\"stylesheet\" type =\"text/css\" />", cssFile);
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body>");

                    HtmlEncoder encoder = new HtmlEncoder();
                    encoder.Encode(writer, sections);

                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");
                }
            }
        }

        private static string cssFile = null;
        private static string outputDir = null;
        private static string outputFile = null;
        private static string inputFile = null;
        private static string type = null;

        private static bool ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("--css="))
                {
                    cssFile = arg.Substring("--css=".Length);
                }
                else if (arg.StartsWith("--output-dir="))
                {
                    outputDir = arg.Substring("--output-dir=".Length);
                }
                else if (arg.StartsWith("--type="))
                {
                    type = arg.Substring("--type=".Length);
                }
                else if (arg.StartsWith("--output-file="))
                {
                    outputFile = arg.Substring("--output-file=".Length);
                }
                else
                {
                    inputFile = arg;
                }
            }

            if (string.IsNullOrEmpty(inputFile))
            {
                Console.Error.WriteLine("input file is nothing.");

                ShowCommandline();
                return false;
            }

            if (string.IsNullOrEmpty(outputDir))
            {
                outputDir = Path.GetDirectoryName(inputFile);
            }

            if (string.IsNullOrEmpty(cssFile))
            {
                cssFile = "style/doc.css";
            }

            if (string.IsNullOrEmpty(type))
            {
                type = HTML_TYPE;
            }

            if (string.IsNullOrEmpty(outputFile))
            {
                outputFile = Path.GetFileNameWithoutExtension(inputFile) + "." + type;
            }

            return true;
        }
        private static void ShowCommandline()
        {
            Console.WriteLine(@"> MdocConv [options] inputFile
## Options

--output-dir='Output Directory'
        Output directory. File name is auto generate.

--output-file=='Output File'
        Output file name.

--css='CSS File Name'
        HTML stylesheet file name.

--type='output type'
        Supported format ""html"".
");


            //if (arg.StartsWith("--css="))
            //{
            //    ccsFile = arg.Substring("--ccs=".Length);
            //}
            //else if (arg.StartsWith("--output-dir="))
            //{
            //    outputDir = arg.Substring("--output-dir=".Length);
            //}
            //else if (arg.StartsWith("--type="))
            //{
            //    type = arg.Substring("--type=".Length);
            //}
            //else if (arg.StartsWith("--output-file="))
            //{
            //    outputFile = arg.Substring("--output-file=".Length);
            //}
        }
    }
}
