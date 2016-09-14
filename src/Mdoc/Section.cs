using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdoc
{
    public class Section
    {
    }

    public class ParagraphSection : Section
    {
        public TextElement[] Text;

        public ParagraphSection(TextElement[] text)
        {
            this.Text = text;
        }
    }

    public class HeadSection : Section
    {
        public TextElement[] Text;
        public int Level;

        public HeadSection(TextElement[] text, int level)
        {
            this.Text = text;
            this.Level = level;
        }
    }

    public class HorizonSection : Section
    {
        public HorizonSection()
        {
        }
    }

    public class ListItemSection : Section
    {
        public TextElement[] Text;
        public char Mark;
        public List<ListSection> ChildList = new List<ListSection>();

        public ListItemSection(TextElement[] text, char mark)
        {
            this.Text = text;
            this.Mark = mark;
        }
    }
    public class ListSection : Section
    {
        public ListItemSection[] Items;

        public ListSection(ListItemSection[] items)
        {
            this.Items = items;
        }
    }

    public class OrderListSection : ListSection
    {
        public OrderListSection(ListItemSection[] items) : base(items)
        {
        }
    }

    public class DefinitionListSection : Section
    {
        public DefinitionItemSection[] Items;
        public DefinitionListSection(DefinitionItemSection[] items)
        {
            this.Items = items;
        }
    }

    public class DefinitionItemSection : Section
    {
        public TextElement[] Caption;
        public TextElement[] Data;

        public DefinitionItemSection(TextElement[] caption, TextElement[] data)
        {
            this.Caption = caption;
            this.Data = data;
        }
    }

    public class CodeSection : Section
    {
        public string Text;

        public CodeSection(string text)
        {
            this.Text = text;
        }
    }

    public class QuoteSection : Section
    {
        public Section[] Texts;

        public QuoteSection(Section[] texts)
        {
            this.Texts = texts;
        }
    }

    public class ContentsSection : Section
    {
        public int LevelLower;
        public int LevelUpper;

        public ContentsSection(int levelLower, int levelUpper)
        {
            this.LevelLower = levelLower;
            this.LevelUpper = levelUpper;
        }
    }

    public class ContentsAllSection : Section
    {
        public int LevelLower;
        public int LevelUpper;

        public ContentsAllSection(int levelLower, int levelUpper)
        {
            this.LevelLower = levelLower;
            this.LevelUpper = levelUpper;
        }
    }

    public class TextElement
    {
    }

    public class TextSpan : TextElement
    {
        public string Text;

        public TextSpan(string text)
        {
            this.Text = text;
        }
    }

    public class EmphasisOpenTag : TextElement
    {
    }

    public class EmphasisCloseTag : TextElement
    {
    }

    public class StrongOpenTag : TextElement
    {
    }

    public class StrongCloseTag : TextElement
    {
    }

    public class StrikethroughOpenTag : TextElement
    {
    }
    public class StrikethroughCloseTag : TextElement
    {
    }

    public class SuperscriptOpenTag : TextElement
    {
    }
    public class SuperscriptCloseTag : TextElement
    {
    }

    public class SubscriptOpenTag : TextElement
    {
    }
    public class SubscriptCloseTag : TextElement
    {
    }

    public class SmallOpenTag : TextElement
    {
    }
    public class SmallCloseTag : TextElement
    {
    }

    public class CodeSpan : TextElement
    {
        public string Text;

        public CodeSpan(string text)
        {
            this.Text = text;
        }
    }

    public class HyperlinkSpan : TextElement
    {
        public string Text;
        public string Href;
        public string Title;

        public HyperlinkSpan(string text, string href, string title)
        {
            this.Text = text;
            this.Href = href;
            this.Title = title;
        }
    }

    public class ImageSpan : TextElement
    {
        public string Text;
        public string Source;
        public string Title;

        public ImageSpan(string text, string href, string title)
        {
            this.Text = text;
            this.Source = href;
            this.Title = title;
        }
    }
}
