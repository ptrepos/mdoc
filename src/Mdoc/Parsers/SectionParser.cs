using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mdoc.Parsers
{
    public class SectionParser
    {
        private LineParser lineParser;
        private TextWriter messageWriter = null;

        public SectionParser(TextReader reader)
        {
            this.lineParser = new LineParser(reader);
        }

        public TextWriter MessageWriter
        {
            get { return messageWriter; }
            set { messageWriter = value; }
        }

        public Section[] Parse()
        {
            List<Section> sections = new List<Section>();

            try
            {
                lineParser.Parse();
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message);
            }

            while (lineParser.Type != LineType.EOF)
            {
                try
                {
                    switch (lineParser.Type)
                    {
                        case LineType.EMPTY:
                            lineParser.Parse();
                            continue;
                        case LineType.TEXT:
                            sections.Add(ParseParagraph());
                            break;
                        case LineType.HEAD:
                            sections.Add(new HeadSection(ParseText(lineParser.Text, lineParser.LineCount), lineParser.Level));
                            lineParser.Parse();
                            break;
                        case LineType.HORIZON:
                            sections.Add(new HorizonSection());
                            lineParser.Parse();
                            break;
                        case LineType.LIST_ITEM:
                            sections.Add(ParseListSection(1));
                            break;
                        case LineType.ORDER_LIST_ITEM:
                            sections.Add(ParseOrderListSection(1));
                            break;
                        case LineType.DEFINITION_ITEM:
                            sections.Add(ParseDefinitionListSection());
                            break;
                        case LineType.CODE:
                            sections.Add(ParseCodeSection());
                            break;
                        case LineType.QUOTE:
                            sections.Add(ParseQuoteSection(1));
                            break;
                        case LineType.CONTENTS:
                            sections.Add(new ContentsSection(lineParser.LevelLower, lineParser.LevelUpper));
                            lineParser.Parse();
                            break;
                        case LineType.CONTENTS_ALL:
                            sections.Add(new ContentsAllSection(lineParser.LevelLower, lineParser.LevelUpper));
                            lineParser.Parse();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    WriteMessage(ex.Message);
                    lineParser.Parse();
                }
            }

            return sections.ToArray();
        }

        private void WriteMessage(string message)
        {
            if(messageWriter != null)
                messageWriter.WriteLine(message);
        }

        private ParagraphSection ParseParagraph()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(lineParser.Text);

            while (lineParser.Parse())
            {
                if (lineParser.Type == LineType.TEXT)
                {
                    text.AppendLine(lineParser.Text);
                }
                else
                {
                    break;
                }
            }
            return new ParagraphSection(ParseText(text.ToString(), lineParser.LineCount));
        }

        private ListSection ParseListSection(int level)
        {
            List<ListItemSection> items = new List<ListItemSection>();

            while (lineParser.Type != LineType.EOF)
            {
                if (lineParser.Type == LineType.LIST_ITEM && lineParser.Level == level)
                {
                    items.Add(new ListItemSection(ParseText(lineParser.Text, lineParser.LineCount), lineParser.Mark));
                    lineParser.Parse();
                }
                else if (lineParser.Level > level)
                {
                    ListSection section = null;
                    if (lineParser.Type == LineType.LIST_ITEM)
                        section = ParseListSection(level + 1);
                    else if (lineParser.Type == LineType.ORDER_LIST_ITEM)
                        section = ParseOrderListSection(level + 1);
                    else
                        break;

                    // 要素がない場合は空白のリストアイテムを作成
                    if (items.Count <= 0)
                        items.Add(new ListItemSection(null, '\0'));

                    items[items.Count - 1].ChildList.Add(section);
                }
                else
                {
                    break;
                }
            }
            return new ListSection(items.ToArray());
        }

        private OrderListSection ParseOrderListSection(int level)
        {
            List<ListItemSection> items = new List<ListItemSection>();

            while (lineParser.Type != LineType.EOF)
            {
                if (lineParser.Type == LineType.ORDER_LIST_ITEM && lineParser.Level == level)
                {
                    items.Add(new ListItemSection(ParseText(lineParser.Text, lineParser.LineCount), '\0'));
                    if (lineParser.Parse() == false)
                        return new OrderListSection(items.ToArray());
                }
                else if (lineParser.Level > level)
                {
                    ListSection section = null;
                    if (lineParser.Type == LineType.LIST_ITEM)
                        section = ParseListSection(level + 1);
                    else if (lineParser.Type == LineType.ORDER_LIST_ITEM)
                        section = ParseOrderListSection(level + 1);
                    else
                        break;

                    // 要素がない場合は空白のリストアイテムを作成
                    if (items.Count <= 0)
                        items.Add(new ListItemSection(null, '\0'));

                    items[items.Count - 1].ChildList.Add(section);
                }
                else
                {
                    break;
                }
            }
            return new OrderListSection(items.ToArray());
        }

        private DefinitionListSection ParseDefinitionListSection()
        {
            List<DefinitionItemSection> items = new List<DefinitionItemSection>();

            for (;;)
            {
                if (lineParser.Type != LineType.DEFINITION_ITEM)
                {
                    break;
                }
                string caption = lineParser.Text;

                lineParser.Parse();

                ParagraphSection paragraph = ParseParagraph();

                items.Add(new DefinitionItemSection(ParseText(caption, lineParser.LineCount), paragraph.Text));

                if (SkipEmptyLine() == false)
                {
                    break;
                }
            }

            return new DefinitionListSection(items.ToArray());
        }

        private CodeSection ParseCodeSection()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(lineParser.Text);

            while (lineParser.Parse())
            {
                if (lineParser.Type == LineType.CODE)
                {
                    text.AppendLine(lineParser.Text);
                }
                else
                {
                    break;
                }
            }
            return new CodeSection(text.ToString());
        }

        private QuoteSection ParseQuoteSection(int level)
        {
            List<Section> sections = new List<Section>();

            StringBuilder text = new StringBuilder();

            for (;;) {
                if (lineParser.Type == LineType.QUOTE && lineParser.Level == level)
                {
                    text.AppendLine(lineParser.Text);
                    if (lineParser.Parse() == false)
                        break;
                }
                else if (lineParser.Type == LineType.QUOTE && lineParser.Level > level)
                {
                    if (text.Length > 0)
                    {
                        sections.Add(new ParagraphSection(ParseText(text.ToString(), lineParser.LineCount)));
                        text.Clear();
                    }
                    sections.Add(ParseQuoteSection(level + 1));
                }
                else
                {
                    break;
                }
            }

            if (text.Length > 0)
            {
                sections.Add(new ParagraphSection(ParseText(text.ToString(), lineParser.LineCount)));
            }

            return new QuoteSection(sections.ToArray());
        }

        private bool SkipEmptyLine()
        {
            while (lineParser.Type == LineType.EMPTY)
            {
                if (lineParser.Parse() == false)
                {
                    return false;
                }
            }
            return true;
        }

        private TextElement[] ParseText(string text, int lineCount)
        {
            TextParser parser = new TextParser(text, lineCount);
            return parser.Parse();
        }
    }
}
