using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mdoc.Parsers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mdoc.Parsers.Tests
{
    [TestClass()]
    public class SectionParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            string text = @"# HEAD1
DOCUMENT CREATE.
DOCUMENT CREATE.
DOCUMENT CREATE.

* ITEM1
    * ITEM1-1
* ITEM2
        + ITEM2-1-1
    - ITEM2-2
    + ITEM2-3
* ITEM3
    + ITEM3-1
        - ITEM3-1-1

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
";
            using (StringReader reader = new StringReader(text))
            {
                SectionParser parser = new SectionParser(reader);

                Section[] sections = parser.Parse();
            }
        }
    }
}