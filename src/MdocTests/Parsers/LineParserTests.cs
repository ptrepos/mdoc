using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mdoc.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mdoc.Parsers.Tests
{
    [TestClass()]
    public class LineParserTests
    {
        [TestMethod()]
        public void ParseHeadTest()
        {
            string Text = @"#HEAD1
##HEAD2
###HEAD3
####HEAD4
#####HEAD5
######HEAD6
# HEAD1
## HEAD2
### HEAD3
#### HEAD4
##### HEAD5
###### HEAD6
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "HEAD1");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "HEAD2");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 3);
                Assert.AreEqual(parser.Text, "HEAD3");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 4);
                Assert.AreEqual(parser.Text, "HEAD4");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 5);
                Assert.AreEqual(parser.Text, "HEAD5");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 6);
                Assert.AreEqual(parser.Text, "HEAD6");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "HEAD1");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "HEAD2");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 3);
                Assert.AreEqual(parser.Text, "HEAD3");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 4);
                Assert.AreEqual(parser.Text, "HEAD4");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 5);
                Assert.AreEqual(parser.Text, "HEAD5");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HEAD);
                Assert.AreEqual(parser.Level, 6);
                Assert.AreEqual(parser.Text, "HEAD6");

                Assert.IsFalse(parser.Parse());
            }

        }
        [TestMethod()]
        public void ParseTextTest()
        {
            string Text = @"
AAAA

BBBB

CCCC
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "AAAA");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "BBBB");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "CCCC");

                Assert.IsFalse(parser.Parse());
            }

        }

        [TestMethod()]
        public void ParseHorizonTest()
        {
            string Text = @"
---

--------
-------------------------------------------
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HORIZON);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HORIZON);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.HORIZON);

                Assert.IsFalse(parser.Parse());
            }

        }

        [TestMethod()]
        public void ParseListTest()
        {
            string Text = @"
XXXXXXXXXXXXXXXXXX
* DATA1
* DATA2
    * DATA2-1
    * DATA2-2
        * DATA2-2-1
        + DATA2-2-2
        - DATA2-2-3
* DATA3
+ DATA4
- DATA5
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "XXXXXXXXXXXXXXXXXX");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA1");
                Assert.AreEqual(parser.Mark, '*');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA2");
                Assert.AreEqual(parser.Mark, '*');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "DATA2-1");
                Assert.AreEqual(parser.Mark, '*');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "DATA2-2");
                Assert.AreEqual(parser.Mark, '*');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 3);
                Assert.AreEqual(parser.Text, "DATA2-2-1");
                Assert.AreEqual(parser.Mark, '*');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 3);
                Assert.AreEqual(parser.Text, "DATA2-2-2");
                Assert.AreEqual(parser.Mark, '+');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 3);
                Assert.AreEqual(parser.Text, "DATA2-2-3");
                Assert.AreEqual(parser.Mark, '-');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA3");
                Assert.AreEqual(parser.Mark, '*');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA4");
                Assert.AreEqual(parser.Mark, '+');

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA5");
                Assert.AreEqual(parser.Mark, '-');

                Assert.IsFalse(parser.Parse());
            }

        }
        [TestMethod()]
        public void ParseOrderListTest()
        {
            string Text = @"
XXXXXXXXXXXXXXXXXX
1. DATA1
2. DATA2
    99999. DATA2-1
    99999. DATA2-2
3. DATA3
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "XXXXXXXXXXXXXXXXXX");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.ORDER_LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA1");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.ORDER_LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA2");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.ORDER_LIST_ITEM);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "DATA2-1");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.ORDER_LIST_ITEM);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "DATA2-2");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.ORDER_LIST_ITEM);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "DATA3");

                Assert.IsFalse(parser.Parse());
            }

        }
        [TestMethod()]
        public void ParseDefinitionListTest()
        {
            string Text = @"
XXXXXXXXXXXXXXXXXX
:DATA1
DATA1DATA1DATA1DATA1DATA1DATA1
DATA1DATA1DATA1DATA1DATA1DATA1
:  DATA2
DATA2DATA2DATA2DATA2DATA2
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "XXXXXXXXXXXXXXXXXX");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.DEFINITION_ITEM);
                Assert.AreEqual(parser.Text, "DATA1");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "DATA1DATA1DATA1DATA1DATA1DATA1");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "DATA1DATA1DATA1DATA1DATA1DATA1");
                

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.DEFINITION_ITEM);
                Assert.AreEqual(parser.Text, "DATA2");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "DATA2DATA2DATA2DATA2DATA2");

                Assert.IsFalse(parser.Parse());
            }

        }
        [TestMethod()]
        public void ParseCodeTest()
        {
            string Text = @"
zzzzzzzzzzzz
    ***********************
    -----------------------
    ppppppppppppppppppppppp
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "zzzzzzzzzzzz");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.CODE);
                Assert.AreEqual(parser.Text, "***********************");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.CODE);
                Assert.AreEqual(parser.Text, "-----------------------");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.CODE);
                Assert.AreEqual(parser.Text, "ppppppppppppppppppppppp");

                Assert.IsFalse(parser.Parse());
            }

        }

        [TestMethod()]
        public void ParseQuoteTest()
        {
            string Text = @"
zzzzzzzzzzzz
>AAAAAAAAAAAAAAA
> AAAAAAAAAAAAAAA
> BBBBBBBBBBBBBBB
> > CCCCCCCCCCCCCCC
> > DDDDDDDDDDDDDDD
> EEEEEEEEEEEEEEE
";

            using (StringReader reader = new StringReader(Text))
            {
                LineParser parser = new LineParser(reader);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.EMPTY);

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, "zzzzzzzzzzzz");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.TEXT);
                Assert.AreEqual(parser.Text, ">AAAAAAAAAAAAAAA");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.QUOTE);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "AAAAAAAAAAAAAAA");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.QUOTE);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "BBBBBBBBBBBBBBB");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.QUOTE);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "CCCCCCCCCCCCCCC");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.QUOTE);
                Assert.AreEqual(parser.Level, 2);
                Assert.AreEqual(parser.Text, "DDDDDDDDDDDDDDD");

                Assert.IsTrue(parser.Parse());
                Assert.AreEqual(parser.Type, LineType.QUOTE);
                Assert.AreEqual(parser.Level, 1);
                Assert.AreEqual(parser.Text, "EEEEEEEEEEEEEEE");

                Assert.IsFalse(parser.Parse());
            }

        }
    }
}