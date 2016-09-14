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
            EMPHASIS,
            STRONG,
            HTML_STRONG,
            HTML_EM,
            HTML_DEL,
            HTML_SUP,
            HTML_SUB,
            HTML_SMALL,
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
                        if (status == Status.STRONG)
                        {
                            if (Match(text, index + 1, '*'))
                            {
                                if (builder.Length > 0)
                                {
                                    elems.Add(new TextSpan(builder.ToString()));
                                    builder.Clear();
                                }

                                elems.Add(new StrongCloseTag());
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

                            elems.Add(new EmphasisCloseTag());
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

                            elems.Add(new StrongOpenTag());

                            Parse(Status.STRONG);
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

                            elems.Add(new EmphasisOpenTag());

                            Parse(Status.EMPHASIS);
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

                    case '<':
                        string element;
                        Dictionary<string, string> attrs;
                        bool closed;

                        if (ParseHtmlTagClose(text, ref index, out element))
                        {
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }

                            if (status == Status.HTML_EM)
                            {
                                if (element.ToLower() == "em")
                                {
                                    elems.Add(new EmphasisCloseTag());
                                    return;
                                }
                            }
                            else if (status == Status.HTML_STRONG)
                            {
                                if (element.ToLower() == "strong")
                                {
                                    elems.Add(new StrongCloseTag());
                                    return;
                                }
                            }
                            else if (status == Status.HTML_DEL)
                            {
                                if (element.ToLower() == "del")
                                {
                                    elems.Add(new StrikethroughCloseTag());
                                    return;
                                }
                            }
                            else if (status == Status.HTML_SUP)
                            {
                                if (element.ToLower() == "sup")
                                {
                                    elems.Add(new SuperscriptCloseTag());
                                    return;
                                }
                            }
                            else if (status == Status.HTML_SUB)
                            {
                                if (element.ToLower() == "sub")
                                {
                                    elems.Add(new SubscriptCloseTag());
                                    return;
                                }
                            }
                            else if (status == Status.HTML_SMALL)
                            {
                                if (element.ToLower() == "small")
                                {
                                    elems.Add(new SmallCloseTag());
                                    return;
                                }
                            }

                            throw new MdocParseException("予期せぬHTMLタグを検出しました。", line);
                        }
                        else if (ParseHtmlTagOpen(text, ref index, out closed, out element, out attrs))
                        {
                            if (builder.Length > 0)
                            {
                                elems.Add(new TextSpan(builder.ToString()));
                                builder.Clear();
                            }
                            element = element.ToLower();
                            if (element == "em")
                            {
                                elems.Add(new EmphasisOpenTag());

                                if (closed)
                                {
                                    elems.Add(new EmphasisCloseTag());
                                }
                                else
                                {
                                    Parse(Status.HTML_EM);
                                }
                            }
                            else if (element == "strong")
                            {
                                elems.Add(new StrongOpenTag());

                                if (closed)
                                {
                                    elems.Add(new StrongCloseTag());
                                }
                                else
                                {
                                    Parse(Status.HTML_STRONG);
                                }
                            }
                            else if (element == "del")
                            {
                                elems.Add(new StrikethroughOpenTag());

                                if (closed)
                                {
                                    elems.Add(new StrikethroughCloseTag());
                                }
                                else
                                {
                                    Parse(Status.HTML_DEL);
                                }
                            }
                            else if (element == "sup")
                            {
                                elems.Add(new SuperscriptOpenTag());

                                if (closed)
                                {
                                    elems.Add(new SuperscriptCloseTag());
                                }
                                else
                                {
                                    Parse(Status.HTML_SUP);
                                }
                            }
                            else if (element == "sub")
                            {
                                elems.Add(new SubscriptOpenTag());

                                if (closed)
                                {
                                    elems.Add(new SubscriptCloseTag());
                                }
                                else
                                {
                                    Parse(Status.HTML_SUB);
                                }
                            }
                            else if (element == "small")
                            {
                                elems.Add(new SmallOpenTag());

                                if (closed)
                                {
                                    elems.Add(new SmallCloseTag());
                                }
                                else
                                {
                                    Parse(Status.HTML_SMALL);
                                }
                            }
                            else
                            {
                                throw new MdocParseException("予期せぬHTMLタグを検出しました。", line);
                            }
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
            if (status == Status.STRONG)
            {
                throw new MdocParseException(MessageResource.BoldError, line);
            }
            if (status == Status.HTML_DEL)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
            }
            if (status == Status.HTML_EM)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
            }
            if (status == Status.HTML_STRONG)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
            }
            if (status == Status.HTML_SUP)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
            }
            if (status == Status.HTML_SUB)
            {
                throw new MdocParseException(MessageResource.StrikethroughError, line);
            }
            if (status == Status.HTML_SMALL)
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

        private CodeSpan ParseCode(string text, ref int index)
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

        private HyperlinkSpan ParseUrlHyperlink(string text, ref int index)
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
                        CheckURL(url);

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

        private HyperlinkSpan ParseHyperlink(string text, ref int index)
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

                                string _href = href.ToString();
                                CheckURL(_href);

                                return new HyperlinkSpan(context.ToString(), _href, null);
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

        private ImageSpan ParseImage(string text, ref int index)
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

                                string _href = href.ToString();
                                CheckURL(_href);

                                return new ImageSpan(context.ToString(), _href, null);
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

        private bool ParseHtmlTagOpen(string text, ref int index, out bool closed, out string element, out Dictionary<string,string> attrs)
        {
            if (Match(text, index, '<') == false)
            {
                element = "";
                attrs = null;
                closed = false;


                return false;
            }
            index++;

            SkipWhitespace(text, ref index);

            StringBuilder builder = new StringBuilder();

            while (index < text.Length)
            {
                if (IsHtmlNameChar(text[index]))
                {
                    builder.Append(text[index]);
                    index++;
                }
                else
                {
                    element = builder.ToString();
                    builder.Clear();
                    attrs = new Dictionary<string, string>();

                    while (index < text.Length)
                    {
                        SkipWhitespace(text, ref index);

                        if (text[index] == '>')
                        {
                            index++;
                            closed = false;
                            return true;
                        }
                        else if (text[index] == '/' && Match(text, index + 1, '>') == true)
                        {
                            index += 2;
                            closed = true;
                            return true;
                        }
                        else
                        {
                            while (index < text.Length)
                            {
                                if (IsHtmlNameChar(text[index]))
                                {
                                    builder.Append(text[index]);
                                    index++;
                                }
                                else
                                {
                                    SkipWhitespace(text, ref index);

                                    if (text[index] == '=')
                                    {
                                        index++;

                                        SkipWhitespace(text, ref index);

                                        string value = ParseHtmlAttrString(text, ref index);

                                        attrs.Add(builder.ToString(), value);
                                        builder.Clear();
                                        break;
                                    }
                                    else
                                    {
                                        throw new MdocParseException("MdocのHTML構文が解析できませんでした。", line);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            throw new MdocParseException("MdocのHTML構文が解析できませんでした。", line);
        }

        private bool ParseHtmlTagClose(string text, ref int index, out string element)
        {
            if (Match(text, index, '<') == false || Match(text, index + 1, '/') == false)
            {
                element = null;
                return false;
            }
            index += 2;

            SkipWhitespace(text, ref index);

            StringBuilder builder = new StringBuilder();

            while (index < text.Length)
            {
                if (IsHtmlNameChar(text[index]))
                {
                    builder.Append(text[index]);
                    index++;
                }
                else
                {
                    SkipWhitespace(text, ref index);

                    if (Match(text, index, '>'))
                    {
                        index++;
                        element = builder.ToString().ToLower();
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            throw new MdocParseException("MdocのHTML構文が解析できませんでした。", line);
        }

        private void SkipWhitespace(string text, ref int index)
        {
            while (index < text.Length)
            {
                if (text[index] != ' ' && text[index] != '\t')
                {
                    break;
                }
                index++;
            }
        }

        private string ParseHtmlAttrString(string text, ref int index)
        {
            if (text[index] != '"')
            {
                throw new MdocParseException("MdocのHTML構文が解析できませんでした。", line);
            }
            index++;

            StringBuilder builder = new StringBuilder();

            while (index < text.Length)
            {
                if (text[index] == '"')
                {
                    index++;

                    return builder.ToString();
                }
                else
                {
                    builder.Append(text[index]);
                    index++;
                }
            }

            throw new MdocParseException("MdocのHTML構文が解析できませんでした。", line);
        }

        private bool IsHtmlNameChar(char c)
        {
            return ('a' <= c && c <= 'z') ||
                   ('A' <= c && c <= 'Z') ||
                   ('A' <= c && c <= '9') ||
                   (c == '_' || c == '-');
        }

        private void CheckURL(string url)
        {
            if (url.StartsWith("javascript:"))
            {
                throw new MdocParseException(MessageResource.UrlCheckJavascript, line);
            }
            if (url.StartsWith("vbscript:"))
            {
                throw new MdocParseException(MessageResource.UrlCheckVbScript, line);
            }
        }
    }
}
