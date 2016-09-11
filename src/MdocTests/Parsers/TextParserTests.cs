using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mdoc.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdoc.Parsers.Tests
{
    [TestClass()]
    public class TextParserTests
    {

        [TestMethod()]
        public void ParseTest()
        {
            string text = @"*AAAAAA* **BBBBB**~~BBBBB~~";

            TextParser parser = new TextParser(text, 100);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(EmphasisStartTag));
            Assert.IsInstanceOfType(elems[1], typeof(TextSpan));
            Assert.IsInstanceOfType(elems[2], typeof(EmphasisEndTag));
            Assert.IsInstanceOfType(elems[3], typeof(TextSpan));
            Assert.IsInstanceOfType(elems[4], typeof(BoldStartTag));
            Assert.IsInstanceOfType(elems[5], typeof(TextSpan));
            Assert.IsInstanceOfType(elems[6], typeof(BoldEndTag));
            Assert.IsInstanceOfType(elems[7], typeof(StrikethroughStartTag));
            Assert.IsInstanceOfType(elems[8], typeof(TextSpan));
            Assert.IsInstanceOfType(elems[9], typeof(StrikethroughEndTag));
        }

        [TestMethod()]
        public void ParseEscapeTest()
        {
            string text = @"\*\*\^\^\~\\";

            TextParser parser = new TextParser(text, 0);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(TextSpan));
            Assert.AreEqual(((TextSpan)elems[0]).Text, "**^^~\\");
        }

        [TestMethod()]
        public void ParseCodeTest()
        {
            string text = @"`aaaa**~~**aaaaa**`";

            TextParser parser = new TextParser(text, 0);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(CodeSpan));
            Assert.AreEqual(((CodeSpan)elems[0]).Text, "aaaa**~~**aaaaa**");
        }
    }
}