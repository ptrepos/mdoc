using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdoc.Parsers
{
    public class TextParser
    {
        private string text;

        public TextParser(string text)
        {
            this.text = text;
        }

        public TextElement[] Parse()
        {
            bool bold = false;
            bool hardbold = false;
            bool strikethrough = false;

            List<TextElement> elems = new List<TextElement>();

            StringBuilder builder = new StringBuilder();

            int i = 0;
            while (i < text.Length)
            {
                switch (text[i])
                {
                    case '*':
                        if (Match(text, i + 1, '*'))
                        {
                            // **
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            if (!hardbold)
                            {
                                elems.Add(new HardboldStartTag());
                            }
                            else
                            {
                                elems.Add(new HardboldEndTag());
                            }

                            hardbold = !hardbold;

                            i += 2;
                        }
                        else
                        {
                            // *
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            if (!bold)
                            {
                                elems.Add(new BoldStartTag());
                            }
                            else
                            {
                                elems.Add(new BoldEndTag());
                            }

                            bold = !bold;

                            i += 1;
                        }
                        break;

                    case '~':
                        if (Match(text, i + 1, '~'))
                        {
                            // ~~
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            if (!strikethrough)
                            {
                                elems.Add(new StrikethroughStartTag());
                            }
                            else
                            {
                                elems.Add(new StrikethroughEndTag());
                            }

                            strikethrough = !strikethrough;

                            i += 2;
                        }
                        else
                        {
                            builder.Append(text[i]);
                            i += 1;
                        }

                        break;
                    case '`':
                        if (builder.Length > 0)
                        {
                            elems.Add(new TextSpan(builder.ToString()));
                            builder.Clear();
                        }

                        // `XXXXXXXXXXXXXXXXXXX`
                        CodeSpan codeSpan = ParseCode(text, ref i);
                        if (codeSpan != null)
                            elems.Add(codeSpan);

                        break;
                    case '\\':
                        if (i + 1 < text.Length)
                        {
                            // \?
                            builder.Append(text[i + 1]);
                            i += 2;
                        }
                        else
                        {
                            // \マークは消える
                            i += 1;
                        }
                        break;
                    case '[':
                        if (Match(text, i + 1, '['))
                        {
                            // [[
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            HyperlinkTag tag = ParseHyperlinkAuto(text, ref i);
                            if (tag != null)
                                elems.Add(tag);
                            continue;
                        }
                        else
                        {
                            // [
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            HyperlinkTag tag = ParseHyperlink(text, ref i);
                            if (tag != null)
                                elems.Add(tag);
                        }
                        break;

                    default:
                        builder.Append(text[i]);
                        i += 1;
                        break;
                }
            }

            if (builder.Length > 0)
            {
                elems.Add(new TextSpan(builder.ToString()));
                builder.Clear();
            }

            if (bold)
            {
                throw new MdocParseException("inline element error: '*' is not closed.", 0);
            }
            if (hardbold)
            {
                throw new MdocParseException("inline element error: '**' is not closed.", 0);
            }
            if (strikethrough)
            {
                throw new MdocParseException("inline element error: '~~' is not closed.", 0);
            }

            return elems.ToArray();
        }

        private bool Match(string line, int index, char c)
        {
            if (index >= line.Length)
            {
                return false;
            }
            return line[index] == c;
        }

        CodeSpan ParseCode(string text, ref int index)
        {
            StringBuilder context = new StringBuilder();

            if (Match(text, index, '`') == false)
            {
                throw new MdocParseException("inline element error: '`' is not opened.", 0);
            }
            index++;

            while (index < text.Length)
            {
                if (text[index] == '`')
                {
                    index += 1;
                    return new CodeSpan(context.ToString());
                }
                else if (text[index] == '\\')
                {
                    if (index + 1 < text.Length)
                    {
                        context.Append(text[index + 1]);
                        index += 2;
                    }
                }
                else
                {
                    context.Append(text[index]);
                    index += 1;
                }
            }

            throw new MdocParseException("inline element error: '`' is not closed.", 0);
        }

        HyperlinkTag ParseHyperlinkAuto(string text, ref int index)
        {
            StringBuilder context = new StringBuilder();
            StringBuilder href = new StringBuilder();

            if (Match(text, index, '[') == false ||
                Match(text, index + 1, '[') == false)
            {
                throw new MdocParseException("inline element error: Hyperlink format error.", 0);
            }
            index += 2;

            while (index < text.Length)
            {
                if (text[index] == ']')
                {
                    if (Match(text, index + 1, ']'))
                    {
                        string url = context.ToString();
                        index += 2;
                        return new HyperlinkTag(url, url, null);
                    }
                    else
                    {
                        index++;
                    }
                }
                else if (text[index] == '\\')
                {
                    if (index + 1 < text.Length)
                    {
                        context.Append(text[index + 1]);
                        index += 2;
                    }
                }
                else
                {
                    context.Append(text[index]);
                    index += 1;
                }
            }

            throw new MdocParseException("inline element error: Hyperlink format error.", 0);
        }

        HyperlinkTag ParseHyperlink(string text, ref int index)
        {
            StringBuilder context = new StringBuilder();
            StringBuilder href = new StringBuilder();

            if (Match(text, index,'[') == false)
            {
                throw new MdocParseException("inline element error: Hyperlink format error.", 0);
            }
            index++;
            
            while (index < text.Length)
            {
                if (text[index] == ']')
                {
                    index++;

                    if (Match(text, index, '('))
                    {
                        index++;

                        while (index < text.Length)
                        {
                            if (text[index] == ')')
                            {
                                index++;

                                return new HyperlinkTag(context.ToString(), href.ToString(), null);
                            }
                            else if (text[index] == '\\')
                            {
                                if (index + 1 < text.Length)
                                {
                                    href.Append(text[index + 1]);
                                    index += 2;
                                }
                            }
                            else
                            {
                                href.Append(text[index]);
                                index++;
                            }
                        }
                    }
                    else
                    {
                        throw new MdocParseException("inline element error: Hyperlink format error.", 0);
                    }
                }
                else if (text[index] == '\\')
                {
                    if (index + 1 < text.Length)
                    {
                        context.Append(text[index + 1]);
                        index += 2;
                    }
                }
                else
                {
                    context.Append(text[index]);
                    index++;
                }
            }

            throw new MdocParseException("inline element error: Hyperlink format error.", 0);
        }
    }
}
