using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mdoc;
using Mdoc.Parsers;
using Mdoc.Encoders;
using System;
using System.IO;

namespace Mdoc.Tests
{
    [TestClass()]
    public class HtmlEncoderTests
    {
        [TestMethod()]
        public void EncodeTest()
        {
            string text = @"# HEAD1
DOCUMENT CREATE.
DOCUMENT CREATE.
DOCUMENT CREATE.

## HEAD2
### HEAD3
* ITEM1
    * ITEM1-1
* ITEM2
        + ITEM2-1-1
    - ITEM2-2
    + ITEM2-3
* ITEM3
    + ITEM3-1
        - ITEM3-1-1
XXXXXXXXXXXXXXXXX
1. ITEM1
    * ITEM1-1
    1. ITEM1-2
2. ITEM2
    1. ITEM2-1
    2. ITEM2-2
3. ITEM3
    * ITEM3-1
    * ITEM3-2
    * ITEM3-3

    CODE1
    CODE2
    CODE3

> 1DATA1
> 1DATA2
> 1DATA3

> > 2DATA1
> > 2DATA2
> > 2DATA3

:CAPTION1
CONTEXT CONTEXT CONTEXT
:CAPTION2
CONTEXT1 CONTEXT1 CONTEXT1
";
            Section[] sections;
            using (StringReader reader = new StringReader(text))
            {
                SectionParser parser = new SectionParser(reader);

                sections = parser.Parse();
            }

            HtmlEncoder encoder = new HtmlEncoder();

            StringWriter writer = new StringWriter();
            encoder.Encode(writer, sections);

            string ccc = writer.ToString();
        }
    }
}