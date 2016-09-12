using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mdoc.Parsers
{
    public enum LineType
    {
        EOF,
        EMPTY,
        TEXT,
        HEAD,
        HORIZON,
        LIST_ITEM,
        ORDER_LIST_ITEM,
        DEFINITION_ITEM,
        INDENTED_TEXT,
        QUOTE,
        CONTENTS,
        CONTENTS_ALL
    }

    public class LineParser
    {
        public TextReader reader;
        public int TabSize = 4;

        public LineType Type;
        public string Text;
        public int Level;
        public char Mark;
        public int LevelLower;
        public int LevelUpper;

        public int LineCount = 0;

        public LineParser(TextReader reader)
        {
            this.reader = reader;
        }

        public bool Parse()
        {
            LineType previousType = this.Type;

            this.Type = LineType.EOF;
            this.Text = null;
            this.Level = 0;
            this.LevelLower = 0;
            this.LevelUpper = 0;
            this.Mark = '\0';

            string line = reader.ReadLine();
            if (line == null)
            {
                this.Type = LineType.EOF;
                return false;
            }
            LineCount++;

            // EMPTY LINE
            if (line.Length <= 0)
            {
                this.Type = LineType.EMPTY;
                return true;
            }

            switch (line[0])
            {
                case '#':
                    if (ParseHead(line))
                    {
                        return true;
                    }
                    break;
                case '-':
                    if (ParseHorizon(line))
                    {
                        return true;
                    }
                    if (ParseListItem(line))
                    {
                        return true;
                    }
                    break;
                case '+':
                    if (ParseListItem(line))
                    {
                        return true;
                    }
                    break;
                case '*':
                    if (ParseListItem(line))
                    {
                        return true;
                    }
                    break;
                case '[':
                    if (ParseContents(line))
                    {
                        return true;
                    }
                    else if (ParseContentsAll (line))
                    {
                        return true;
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (ParseOrderListItem(line))
                    {
                        return true;
                    }
                    break;
                case ':':
                    if (ParseDefinitionItem(line))
                    {
                        return true;
                    }
                    break;
                case ' ':
                case '\t':
                    if (previousType == LineType.LIST_ITEM || previousType == LineType.ORDER_LIST_ITEM)
                    {
                        if (ParseListItem(line))
                        {
                            return true;
                        }
                        if (ParseOrderListItem(line))
                        {
                            return true;
                        }
                    }
                    if (ParseIndentedText(line))
                    {
                        return true;
                    }
                    break;
                case '>':
                    if (ParseQuote(line))
                    {
                        return true;
                    }
                    break;
            }

            this.Type = LineType.TEXT;
            this.Text = line;

            return true;
        }

        private bool ParseHead(string line)
        {
            int level = 0;
            int i;
            for (i = 0; i < line.Length && i < 6; i++) {
                if (line[i] == '#')
                    level++;
                else
                    break;
            }

            i = SkipWhitespace(line, i);

            this.Type = LineType.HEAD;
            this.Text = line.Substring(i);
            this.Level = level;

            return true;
        }

        private bool ParseHorizon(string line)
        {
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '-')
                    count++;
                else
                    return false;
            }

            this.Type = LineType.HORIZON;

            return true;
        }

        private bool ParseListItem(string line)
        {
            int i = 0;
            int level = CountIndent(line, ref i) + 1;

            if (i + 2 > line.Length || !IsListMarkChar(line[i]) || line[i + 1] != ' ')
            {
                return false;
            }
            char markChar = line[i];

            i += 2;

            i = SkipWhitespace(line, i);

            this.Type = LineType.LIST_ITEM;
            this.Level = level;
            this.Text = line.Substring(i);
            this.Mark = markChar;

            return true;
        }

        private bool IsListMarkChar(char c)
        {
            return c == '*' || c == '+' || c == '-';
        }

        private bool ParseOrderListItem(string line)
        {
            int i = 0;
            int level = CountIndent(line, ref i) + 1;

            if (i + 1 > line.Length || !('0' <= line[i] && line[i] <= '9'))
            {
                return false;
            }
            i++;

            for (; i < line.Length; i++)
            {
                if ('0' <= line[i] && line[i] <= '9')
                {
                }
                else if(line[i] == '.')
                {
                    if (i + 1 < line.Length && line[i+1] == ' ')
                    {
                        i = SkipWhitespace(line, i + 1);

                        this.Type = LineType.ORDER_LIST_ITEM;
                        this.Text = line.Substring(i);
                        this.Level = level;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool ParseDefinitionItem(string line)
        {
            if (1 <= line.Length && line[0] == ':')
            {
                int i = SkipWhitespace(line, 1);

                this.Type = LineType.DEFINITION_ITEM;
                this.Text = line.Substring(i);

                return true;
            }

            return false;
        }

        private bool ParseIndentedText(string line)
        {
            int i = 0;
            if ((i + 1 <= line.Length && line[i] == '\t'))
            {
                i++;
            }
            else if (
              (i + 4 <= line.Length && line[i] == ' ' && line[i + 1] == ' ' && line[i + 2] == ' ' && line[i + 3] == ' '))
            {
                i += 4;
            }
            else
                return false;

            this.Type = LineType.INDENTED_TEXT;
            this.Text = line.Substring(i);

            return true;
        }

        private bool ParseQuote(string line)
        {
            int level = 0;
            int i = 0;
            while (i < line.Length) {
                if (i + 2 <= line.Length && line[i] == '>' && line[i + 1] == ' ')
                {
                    level++;
                    i += 2;
                }
                else
                {
                    break;
                }
            }

            if (level <= 0)
                return false;

            this.Type = LineType.QUOTE;
            this.Level = level;
            this.Text = line.Substring(i);

            return true;
        }

        private bool ParseContents(string line)
        {
            if (!line.StartsWith("[:contents:"))
            {
                return false;
            }
            int i = "[:contents:".Length;

            StringBuilder levelLower = new StringBuilder();
            StringBuilder levelUpper = new StringBuilder();
            while (i < line.Length)
            {
                if ('0' <= line[i] && line[i] <= '9')
                {
                    levelLower.Append(line[i]);
                    i++;
                }
                else if (line[i] == '-')
                {
                    if (levelLower.Length <= 0)
                    {
                        throw new MdocParseException(MessageResource.ContentsError, LineCount);
                    }
                    i++;

                    while (i < line.Length)
                    {
                        if ('0' <= line[i] && line[i] <= '9')
                        {
                            levelUpper.Append(line[i]);
                            i++;
                        }
                        else if (line[i] == ']')
                        {
                            i++;
                            if (i >= line.Length)
                            {
                                if (levelUpper.Length <= 0)
                                {
                                    throw new MdocParseException(MessageResource.ContentsError, LineCount);
                                }

                                this.Type = LineType.CONTENTS;
                                this.LevelLower = int.Parse(levelLower.ToString());
                                this.LevelUpper = int.Parse(levelUpper.ToString());

                                return true;
                            }
                            break;
                        }
                        else
                        {
                            throw new MdocParseException(MessageResource.ContentsError, LineCount);
                        }
                    }
                    break;
                }
                else
                {
                    throw new MdocParseException(MessageResource.ContentsError, LineCount);
                }
            }

            throw new MdocParseException(MessageResource.ContentsError, LineCount);
        }
        private bool ParseContentsAll(string line)
        {
            if (!line.StartsWith("[:contents-all:"))
            {
                return false;
            }
            int i = "[:contents-all:".Length;

            StringBuilder levelLower = new StringBuilder();
            StringBuilder levelUpper = new StringBuilder();
            while (i < line.Length)
            {
                if ('0' <= line[i] && line[i] <= '9')
                {
                    levelLower.Append(line[i]);
                    i++;
                }
                else if (line[i] == '-')
                {
                    if (levelLower.Length <= 0)
                    {
                        throw new MdocParseException(MessageResource.ContentsAllError, LineCount);
                    }
                    i++;

                    while (i < line.Length)
                    {
                        if ('0' <= line[i] && line[i] <= '9')
                        {
                            levelUpper.Append(line[i]);
                            i++;
                        }
                        else if (line[i] == ']')
                        {
                            i++;
                            if (i >= line.Length)
                            {
                                if (levelUpper.Length <= 0)
                                {
                                    throw new MdocParseException(MessageResource.ContentsAllError, LineCount);
                                }

                                this.Type = LineType.CONTENTS_ALL;
                                this.LevelLower = int.Parse(levelLower.ToString());
                                this.LevelUpper = int.Parse(levelUpper.ToString());

                                return true;
                            }
                            break;
                        }
                        else
                        {
                            throw new MdocParseException(MessageResource.ContentsAllError, LineCount);
                        }
                    }
                    break;
                }
                else
                {
                    throw new MdocParseException(MessageResource.ContentsAllError, LineCount);
                }
            }

            throw new MdocParseException(MessageResource.ContentsAllError, LineCount);
        }

        private int CountIndent(string line, ref int index)
        {
            int indent = 0;
            int i = index;
            while (i < line.Length)
            {
                if (line[i] == '\t')
                {
                    indent++;
                    i++;
                }
                else if (i+4 <= line.Length &&
                         (line[i] == ' ' && line[i+1] == ' ' && line[i + 2] == ' ' && line[i + 3] == ' '))
                {
                    indent++;
                    i += 4;
                }
                else
                {
                    break;
                }
            }
            index = i;

            return indent;
        }

        private int SkipWhitespace(string line, int index)
        {
            for (int i = index; i < line.Length; i++) {
                if (line[i] != ' ' && line[i] != '\t')
                    return i;
            }
            return line.Length;
        }
    }
}
