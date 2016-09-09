using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mdoc.Encoders
{
    public class HtmlEncoder
    {
        public void Encode(TextWriter writer, Section[] sections)
        {
            Dictionary<HeadSection,string> headRefs = CreateTableOfContents(sections);

            for (int i = 0; i < sections.Length; i++)
            {
                Section section = sections[i];

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
                    
                    writer.Write("<a name=\"{0}\">", headRefs[s]);
                    writer.Write("<h{0}>", s.Level);
                    WriteText(writer, s.Text);
                    writer.Write("</h{0}>", s.Level);
                    writer.WriteLine("</a>");
                }
                else if (section is HorizonSection)
                {
                    HorizonSection s = (HorizonSection)section;

                    writer.Write("<hr/>");
                }
                else if (section is CodeSection)
                {
                    CodeSection s = (CodeSection)section;

                    writer.Write("<pre class=\"code\"><code>");
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
                    foreach (ListItemSection j in s.Items)
                    {
                        writer.Write("<li>");
                        WriteText(writer, j.Text);
                        Encode(writer, j.ChildList.ToArray());
                        writer.WriteLine("</li>");
                    }
                    writer.WriteLine("</ol>");
                }
                else if (section is ListSection)
                {
                    ListSection s = (ListSection)section;

                    writer.WriteLine("<ul>");
                    foreach (ListItemSection j in s.Items)
                    {
                        writer.Write(String.Format("<li {0}>", GetListClass(j.Mark)));
                        WriteText(writer, j.Text);
                        Encode(writer, j.ChildList.ToArray());
                        writer.WriteLine("</li>");
                    }
                    writer.WriteLine("</ul>");
                }
                else if (section is DefinitionListSection)
                {
                    DefinitionListSection s = (DefinitionListSection)section;

                    writer.Write("<dl>");
                    foreach (DefinitionItemSection j in s.Items)
                    {
                        writer.Write("<dt>");
                        WriteText(writer, j.Caption);
                        writer.WriteLine("</dt>");
                        writer.Write("<dd>");
                        WriteText(writer, j.Data);
                        writer.WriteLine("</dd>");
                    }
                    writer.WriteLine("</dl>");
                }
                else if (section is DefinitionListSection)
                {
                    DefinitionListSection s = (DefinitionListSection)section;

                    writer.Write("<dl>");
                    foreach (DefinitionItemSection j in s.Items)
                    {
                        writer.Write("<dt>");
                        WriteText(writer, j.Caption);
                        writer.WriteLine("</dt>");
                        writer.Write("<dd>");
                        WriteText(writer, j.Data);
                        writer.WriteLine("</dd>");
                    }
                    writer.WriteLine("</dl>");
                }
                else if (section is ContentsSection)
                {
                    ContentsSection s = (ContentsSection)section;

                    List<ContentItem> contents
                        = ContentsTableGenerator.Generate(sections, i);

                    WriteContents(writer, contents, headRefs);

                }
                else if (section is ContentsAllSection)
                {
                    ContentsAllSection s = (ContentsAllSection)section;

                    List<ContentItem> contents
                        = ContentsTableGenerator.GenerateAll(sections);

                    WriteContents(writer, contents, headRefs);

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
                else if (i is HyperlinkTag)
                {
                    HyperlinkTag s = (HyperlinkTag)i;
                    writer.Write("<a href=\"{1}\">{0}</a>", Escape(s.Text), Escape(s.Href));
                }
            }
        }

        private void WriteContents(TextWriter writer, List<ContentItem> contents, Dictionary<HeadSection,string> headRefs)
        {
            writer.WriteLine("<ul class=\"table_of_contents\">");
            foreach (ContentItem i in contents)
            {
                writer.Write("<li>");
                if (i.Head != null)
                {
                    writer.Write("<a href=\"#{0}\">", headRefs[i.Head]);
                    WriteText(writer, i.Head.Text);
                    writer.Write("</a>");
                }
                WriteContents(writer, i.Items, headRefs);
                writer.WriteLine("</li>");
            }
            writer.WriteLine("</ul>");
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

                    contents.Add(s, GetString(s.Text));
                }
            }

            return contents;
        }

        private string GetString(TextElement[] texts)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TextElement i in texts)
            {
                if (i is TextSpan)
                {
                    TextSpan s = (TextSpan)i;

                    builder.Append(Escape(s.Text));
                }
                else if (i is CodeSpan)
                {
                    CodeSpan s = (CodeSpan)i;

                    builder.Append(Escape(s.Text));
                }
                else if (i is HyperlinkTag)
                {
                    HyperlinkTag s = (HyperlinkTag)i;
                    builder.Append(Escape(s.Text));
                }
            }
            return builder.ToString();
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

    }
}
