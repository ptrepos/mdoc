using System;
using System.Collections.Generic;
using System.IO;

namespace Mdoc
{
    public class MarkdownEncoder
    {
        public void Encode(TextWriter writer, Section[] sections)
        {
            foreach (Section section in sections)
            {
                if (section is ParagraphSection)
                {
                    ParagraphSection s = (ParagraphSection)section;

                    writer.WriteLine(s.Text);
                    writer.WriteLine();
                }
                else if (section is HeadSection)
                {
                    HeadSection s = (HeadSection)section;

                    writer.WriteLine();
                    for (int i = s.Level; i <= 6; i++) {
                        writer.Write("#");
                    }
                    writer.Write(" ");
                    writer.WriteLine(s.Text);
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
                    writer.Write(s.Text);
                    writer.Write("</code></pre>");
                }
                else if (section is QuoteSection)
                {
                    QuoteSection s = (QuoteSection)section;

                    writer.Write("<blockquote>");
                    Encode(writer, s.Texts);
                    writer.Write("</blockquote>");
                }
                else if (section is OrderListSection)
                {
                    OrderListSection s = (OrderListSection)section;

                    writer.Write("<ol>");
                    foreach (ListItemSection i in s.Items)
                    {
                        writer.Write("<li>");
                        writer.Write(i.Text);
                        Encode(writer, i.ChildList.ToArray());
                        writer.Write("</li>");
                    }
                    writer.Write("</ol>");
                }
                else if (section is ListSection)
                {
                    ListSection s = (ListSection)section;

                    writer.Write("<ul>");
                    foreach (ListItemSection i in s.Items)
                    {
                        writer.Write(String.Format("<li {0}>", GetListClass(i.Mark)));
                        writer.Write(i.Text);
                        Encode(writer, i.ChildList.ToArray());
                        writer.Write("</li>");
                    }
                    writer.Write("</ul>");
                }
                else if (section is DefinitionListSection)
                {
                    DefinitionListSection s = (DefinitionListSection)section;

                    writer.Write("<dl>");
                    foreach (DefinitionItemSection i in s.Items)
                    {
                        writer.Write("<dt>");
                        writer.Write(i.Caption);
                        writer.Write("</dt>");
                        writer.Write("<dd>");
                        writer.Write(i.Data);
                        writer.Write("</dd>");
                    }
                    writer.Write("</dl>");
                }
            }
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
    }
}
