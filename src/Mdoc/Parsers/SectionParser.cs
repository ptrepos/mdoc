using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mdoc.Parsers
{
    public class SectionParser
    {
        private LineParser parser;

        public SectionParser(TextReader reader)
        {
            parser = new LineParser(reader);
        }

        public Section[] Parse()
        {
            List<Section> sections = new List<Section>();

            parser.Parse();

            while (parser.Type != LineType.EOF) {
                switch (parser.Type)
                {
                    case LineType.EMPTY:
                        parser.Parse();
                        continue;
                    case LineType.TEXT:
                        sections.Add(ParseParagraph());
                        break;
                    case LineType.HEAD:
                        sections.Add(new HeadSection(ParseText(parser.Text), parser.Level));
                        parser.Parse();
                        break;
                    case LineType.HORIZON:
                        sections.Add(new HorizonSection());
                        parser.Parse();
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
                        sections.Add(new ContentsSection(parser.LevelLower, parser.LevelUpper));
                        parser.Parse();
                        break;
                    case LineType.CONTENTS_ALL:
                        sections.Add(new ContentsAllSection(parser.LevelLower, parser.LevelUpper));
                        parser.Parse();
                        break;
                }
            }

            return sections.ToArray();
        }

        private ParagraphSection ParseParagraph()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(parser.Text);

            while (parser.Parse())
            {
                if (parser.Type == LineType.TEXT)
                {
                    text.AppendLine(parser.Text);
                }
                else
                {
                    break;
                }
            }
            return new ParagraphSection(ParseText(text.ToString()));
        }

        private ListSection ParseListSection(int level)
        {
            List<ListItemSection> items = new List<ListItemSection>();

            while (parser.Type != LineType.EOF)
            {
                if (parser.Type == LineType.LIST_ITEM && parser.Level == level)
                {
                    items.Add(new ListItemSection(ParseText(parser.Text), parser.Mark));
                    parser.Parse();
                }
                else if (parser.Level > level)
                {
                    ListSection section = null;
                    if (parser.Type == LineType.LIST_ITEM)
                        section = ParseListSection(level + 1);
                    else if (parser.Type == LineType.ORDER_LIST_ITEM)
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

            while (parser.Type != LineType.EOF)
            {
                if (parser.Type == LineType.ORDER_LIST_ITEM && parser.Level == level)
                {
                    items.Add(new ListItemSection(ParseText(parser.Text), '\0'));
                    if (parser.Parse() == false)
                        return new OrderListSection(items.ToArray());
                }
                else if (parser.Level > level)
                {
                    ListSection section = null;
                    if (parser.Type == LineType.LIST_ITEM)
                        section = ParseListSection(level + 1);
                    else if (parser.Type == LineType.ORDER_LIST_ITEM)
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
                if (parser.Type != LineType.DEFINITION_ITEM)
                {
                    break;
                }
                string caption = parser.Text;

                parser.Parse();

                ParagraphSection paragraph = ParseParagraph();

                items.Add(new DefinitionItemSection(ParseText(caption), paragraph.Text));

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
            text.AppendLine(parser.Text);

            while (parser.Parse())
            {
                if (parser.Type == LineType.CODE)
                {
                    text.AppendLine(parser.Text);
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
                if (parser.Type == LineType.QUOTE && parser.Level == level)
                {
                    text.AppendLine(parser.Text);
                    if (parser.Parse() == false)
                        break;
                }
                else if (parser.Type == LineType.QUOTE && parser.Level > level)
                {
                    if (text.Length > 0)
                    {
                        sections.Add(new ParagraphSection(ParseText(text.ToString())));
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
                sections.Add(new ParagraphSection(ParseText(text.ToString())));
            }

            return new QuoteSection(sections.ToArray());
        }

        private bool SkipEmptyLine()
        {
            while (parser.Type == LineType.EMPTY)
            {
                if (parser.Parse() == false)
                {
                    return false;
                }
            }
            return true;
        }

        private TextElement[] ParseText(string text)
        {
            TextParser parser = new TextParser(text);
            return parser.Parse();
        }
    }
}
