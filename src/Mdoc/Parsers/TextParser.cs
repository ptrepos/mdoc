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
        private int line;

        public TextParser(string text, int line)
        {
            this.text = text;
            this.line = line;
        }

        public TextElement[] Parse()
        {
            bool emphasis = false;
            bool bold = false;
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

                            if (!bold)
                            {
                                elems.Add(new BoldStartTag());
                            }
                            else
                            {
                                elems.Add(new BoldEndTag());
                            }

                            bold = !bold;

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

                            if (!emphasis)
                            {
                                elems.Add(new EmphasisStartTag());
                            }
                            else
                            {
                                elems.Add(new EmphasisEndTag());
                            }

                            emphasis = !emphasis;

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

                            HyperlinkTag tag = ParseUrlHyperlink(text, ref i);
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

            if (emphasis)
            {
                throw new MdocParseException(MessageResource.EmphasisError, line);
            }
            if (bold)
            {
                throw new MdocParseException(MessageResource.BoldError, line);
            }
            if (strikethrough)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
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
                throw new MdocParseException(MessageResource.InlineCodeError, line);
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

            throw new MdocParseException(MessageResource.InlineCodeError, line);
        }

        HyperlinkTag ParseUrlHyperlink(string text, ref int index)
        {
            StringBuilder context = new StringBuilder();
            StringBuilder href = new StringBuilder();

            if (Match(text, index, '[') == false ||
                Match(text, index + 1, '[') == false)
            {
                throw new MdocParseException(MessageResource.UriHyperlinkError, line);
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

            throw new MdocParseException(MessageResource.UriHyperlinkError, line);
        }

        HyperlinkTag ParseHyperlink(string text, ref int index)
        {
            StringBuilder context = new StringBuilder();
            StringBuilder href = new StringBuilder();

            if (Match(text, index,'[') == false)
            {
                throw new MdocParseException(MessageResource.HyperlinkError, line);
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
                        throw new MdocParseException(MessageResource.HyperlinkError, line);
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

            throw new MdocParseException(MessageResource.HyperlinkError, line);
        }
    }
}
