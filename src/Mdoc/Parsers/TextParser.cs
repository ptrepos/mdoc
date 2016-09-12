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

        private int index;
        private List<TextElement> elems = new List<TextElement>();

        public TextParser(string text, int line)
        {
            this.text = text;
            this.line = line;
        }

        private enum Status
        {
            NONE,
            BOLD,
            EMPHASIS,
            STRIKETHROUGH,
        }

        public TextElement[] Parse()
        {
            this.index = 0;
            this.elems.Clear();

            Parse(Status.NONE);

            return elems.ToArray();
        }

        private void Parse(Status status)
        {
            StringBuilder builder = new StringBuilder();

            while (index < text.Length)
            {
                switch (text[index])
                {
                    case '*':
                        if (status == Status.BOLD)
                        {
                            if (Match(text, index + 1, '*'))
                            {
                                if (builder.Length > 0)
                                {
                                    elems.Add(new TextSpan(builder.ToString()));
                                    builder.Clear();
                                }

                                elems.Add(new BoldEndTag());
                                index += 2;
                                return;
                            }
                        }
                        if (status == Status.EMPHASIS)
                        {
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            elems.Add(new EmphasisEndTag());
                            index += 1;
                            return;
                        }

                        if (Match(text, index + 1, '*'))
                        {
                            // **
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            index += 2;

                            elems.Add(new BoldStartTag());

                            Parse(Status.BOLD);
                        }
                        else
                        {
                            // *
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            index += 1;

                            elems.Add(new EmphasisStartTag());

                            Parse(Status.EMPHASIS);
                        }
                        break;

                    case '~':
                        if (status == Status.STRIKETHROUGH)
                        {
                            if (Match(text, index + 1, '~'))
                            {
                                if (builder.Length > 0)
                                {
                                    elems.Add(new TextSpan(builder.ToString()));
                                    builder.Clear();
                                }

                                elems.Add(new StrikethroughEndTag());

                                index += 2;
                                return;
                            }
                        }
                        if (Match(text, index + 1, '~'))
                        {
                            // ~~
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            elems.Add(new StrikethroughStartTag());
                            index += 2;

                            Parse(Status.STRIKETHROUGH);
                        }
                        else
                        {
                            builder.Append(text[index]);
                            index += 1;
                        }

                        break;
                    case '`':
                        if (builder.Length > 0)
                        {
                            elems.Add(new TextSpan(builder.ToString()));
                            builder.Clear();
                        }

                        // `XXXXXXXXXXXXXXXXXXX`
                        CodeSpan codeSpan = ParseCode(text, ref index);
                        if (codeSpan != null)
                            elems.Add(codeSpan);

                        break;
                    case '\\':
                        if (index + 1 < text.Length)
                        {
                            // \?
                            builder.Append(text[index + 1]);
                            index += 2;
                        }
                        else
                        {
                            // \マークは消える
                            index += 1;
                        }
                        break;
                    case '[':
                        if (Match(text, index + 1, '['))
                        {
                            // [[
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            HyperlinkSpan tag = ParseUrlHyperlink(text, ref index);
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

                            HyperlinkSpan tag = ParseHyperlink(text, ref index);
                            if (tag != null)
                                elems.Add(tag);
                        }
                        break;

                    case '!':
                        if (Match(text, index + 1, '['))
                        {
                            // ![
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            ImageSpan tag = ParseImage(text, ref index);
                            if (tag != null)
                                elems.Add(tag);
                            continue;
                        }
                        break;
                    default:
                        builder.Append(text[index]);
                        index += 1;
                        break;
                }
            }

            if (builder.Length > 0)
            {
                elems.Add(new TextSpan(builder.ToString()));
                builder.Clear();
            }

            if (status == Status.EMPHASIS)
            {
                throw new MdocParseException(MessageResource.EmphasisError, line);
            }
            if (status == Status.BOLD)
            {
                throw new MdocParseException(MessageResource.BoldError, line);
            }
            if (status == Status.STRIKETHROUGH)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
            }
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

        HyperlinkSpan ParseUrlHyperlink(string text, ref int index)
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
                        return new HyperlinkSpan(url, url, null);
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

        HyperlinkSpan ParseHyperlink(string text, ref int index)
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

                                return new HyperlinkSpan(context.ToString(), href.ToString(), null);
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

        ImageSpan ParseImage(string text, ref int index)
        {
            StringBuilder context = new StringBuilder();
            StringBuilder href = new StringBuilder();

            if (Match(text, index, '!') == false ||
                Match(text, index + 1, '[') == false)
            {
                throw new MdocParseException(MessageResource.HyperlinkError, line);
            }
            index += 2;

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

                                return new ImageSpan(context.ToString(), href.ToString(), null);
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
