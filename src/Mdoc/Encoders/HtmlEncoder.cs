using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mdoc
{
    public class HtmlEncoder
    {
        public void Encode(TextWriter writer, Section[] sections)
        {
            Dictionary<HeadSection,string> contsntsTable = CreateTableOfContents(sections);

            foreach (Section section in sections)
            {
                if (section is ParagraphSection)
                {
                    ParagraphSection s = (ParagraphSection)section;

                    writer.Write("<p>");
                    WriteText(writer, s.Text);
                    writer.WriteLine("</p>");
                }
                else if (section is HeadSection)
                {
                    HeadSection s = (HeadSection)section;

                    writer.Write("<h{0}>", s.Level);
                    writer.Write("<a name=\"{0}\">", contsntsTable[s]);
                    WriteText(writer, s.Text);
                    writer.Write("</a>");
                    writer.WriteLine("</h{0}>", s.Level);
                }
                else if (section is HorizonSection)
                {
                    HorizonSection s = (HorizonSection)section;

                    writer.Write("<hr/>");
                }
                else if (section is CodeSection)
                {
                    CodeSection s = (CodeSection)section;

                    writer.Write("<pre><code>");
                    writer.Write(Escape(s.Text));
                    writer.WriteLine("</code></pre>");
                }
                else if (section is QuoteSection)
                {
                    QuoteSection s = (QuoteSection)section;

                    writer.Write("<blockquote>");
                    Encode(writer, s.Texts);
                    writer.WriteLine("</blockquote>");
                }
                else if (section is OrderListSection)
                {
                    OrderListSection s = (OrderListSection)section;

                    writer.WriteLine("<ol>");
                    foreach (ListItemSection i in s.Items)
                    {
                        writer.Write("<li>");
                        WriteText(writer, i.Text);
                        Encode(writer, i.ChildList.ToArray());
                        writer.WriteLine("</li>");
                    }
                    writer.WriteLine("</ol>");
                }
                else if (section is ListSection)
                {
                    ListSection s = (ListSection)section;

                    writer.WriteLine("<ul>");
                    foreach (ListItemSection i in s.Items)
                    {
                        writer.Write(String.Format("<li {0}>", GetListClass(i.Mark)));
                        WriteText(writer, i.Text);
                        Encode(writer, i.ChildList.ToArray());
                        writer.WriteLine("</li>");
                    }
                    writer.WriteLine("</ul>");
                }
                else if (section is DefinitionListSection)
                {
                    DefinitionListSection s = (DefinitionListSection)section;

                    writer.Write("<dl>");
                    foreach (DefinitionItemSection i in s.Items)
                    {
                        writer.Write("<dt>");
                        WriteText(writer, i.Caption);
                        writer.WriteLine("</dt>");
                        writer.Write("<dd>");
                        WriteText(writer, i.Data);
                        writer.WriteLine("</dd>");
                    }
                    writer.WriteLine("</dl>");
                }
            }
        }

        private void WriteText(TextWriter writer, TextElement[] elems)
        {
            foreach (TextElement i in elems)
            {
                if (i is TextSpan)
                {
                    TextSpan s = (TextSpan)i;

                    writer.Write(Escape(s.Text));
                }
                else if (i is CodeSpan)
                {
                    CodeSpan s = (CodeSpan)i;

                    writer.Write(Escape(s.Text));
                }
                else if (i is BoldStartTag)
                {
                    writer.Write("<em>");
                }
                else if (i is BoldEndTag)
                {
                    writer.Write("</em>");
                }
                else if (i is HardboldStartTag)
                {
                    writer.Write("<strong>");
                }
                else if (i is HardboldEndTag)
                {
                    writer.Write("</strong>");
                }
                else if (i is StrikethroughStartTag)
                {
                    writer.Write("<strike>");
                }
                else if (i is StrikethroughEndTag)
                {
                    writer.Write("</strike>");
                }
            }
        }

        private string Escape(string text)
        {
            StringBuilder builder = new StringBuilder(text);
            builder.Replace("&", "&amp;");
            builder.Replace("'", "&quot;");
            builder.Replace("<", "&lt;");
            builder.Replace(">", "&gt;");
            return builder.ToString();
        }

        private string GetListClass(char mark)
        {
            if (mark == '+')
                return "class=\"plus\"";
            else if (mark == '-')
                return "class=\"hyphen\"";
            else if (mark == '*')
                return "class=\"aster\"";
            return null;
        }

        private Dictionary<HeadSection, string> CreateTableOfContents(Section[] sections)
        {
            Dictionary<HeadSection, string> contents = new Dictionary<HeadSection, string>();

            int seq = 0;

            foreach (Section section in sections)
            {
                if (section is HeadSection)
                {
                    HeadSection s = (HeadSection)section;

                    contents.Add(s, "head" + seq++);
                }
            }

            return contents;
        }
    }
}
