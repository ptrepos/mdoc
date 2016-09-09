using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdoc.Encoders
{
    public class ContentItem
    {
        public HeadSection Head;
        public List<ContentItem> Items = new List<ContentItem>();

        public ContentItem(HeadSection head)
        {
            this.Head = head;
        }
    }

    public class ContentsTableGenerator
    {
        public static List<ContentItem> Generate(Section[] sections, int index)
        {
            int headLevel = 0;
            for (int i = index; i > 0; i--)
            {
                if (sections[i-1] is HeadSection)
                {
                    HeadSection s = (HeadSection)sections[i-1];
                    headLevel = s.Level;
                    break;
                }
            }
            return Generate(sections, index, headLevel);
        }

        public static List<ContentItem> GenerateAll(Section[] sections)
        {
            return Generate(sections, 0, 0);
        }

        public static List<ContentItem> Generate(Section[] sections, int index, int headLevel)
        {
            List<ContentItem> items = new List<ContentItem>();

            for (int i = index; i < sections.Length; i++)
            {
                if (sections[i] is HeadSection)
                {
                    HeadSection s = (HeadSection)sections[i];
                    if (s.Level <= headLevel)
                    {
                        break;
                    }
                    else
                    {
                        int depth = s.Level - headLevel;

                        List<ContentItem> current = items;
                        for (int j = 0; j < depth; j++)
                        {
                            if (depth - j <= 1)
                            {
                                current.Add(new ContentItem(s));
                                break;
                            }
                            if (current.Count <= 0)
                                current.Add(new ContentItem(null));
                            current = current[current.Count - 1].Items;
                        }
                    }
                }
            }
            return items;
        }
    }
}
