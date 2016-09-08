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
                        if (builder.Length > 0)
                        {
                            elems.Add(new TextSpan(builder.ToString()));
                            builder.Clear();
                        }

                        if (i + 1 < text.Length && text[i + 1] == '*')
                        {
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
                            continue;
                        }
                        else
                        {
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
                            continue;
                        }
                    case '~':
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

                        i += 1;
                        continue;
                    case '`':
                        if (builder.Length > 0)
                        {
                            elems.Add(new TextSpan(builder.ToString()));
                            builder.Clear();
                        }

                        i++;
                        while (i < text.Length)
                        {
                            if (text[i] == '`')
                            {
                                elems.Add(new CodeSpan(builder.ToString()));
                                builder.Clear();
                                i += 1;
                                break;
                            }
                            else if (text[i] == '\\')
                            {
                                if (i + 1 < text.Length)
                                {
                                    builder.Append(text[i + 1]);
                                    i += 2;
                                }
                            }
                            else
                            {
                                builder.Append(text[i]);
                                i += 1;
                            }
                        }

                        continue;
                    case '\\':
                        if (i + 1 < text.Length)
                        {
                            builder.Append(text[i + 1]);
                            i += 2;
                        }
                        else
                        {
                            // \マークは消える
                            i += 1;
                        }
                        break;
                    //case '[':
                    //    if (text.Length > 0)
                    //    {
                    //        elems.Add(new TextSpan(text.ToString()));
                    //        text.Clear();
                    //    }
                    //    break;

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
                elems.Add(new BoldEndTag());
            if (hardbold)
                elems.Add(new HardboldEndTag());
            if (strikethrough)
                elems.Add(new StrikethroughEndTag());

            return elems.ToArray();
        }
    }
}
