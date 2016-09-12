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
        public void ParseBoldEmphasisTest()
        {
            string text = @"***AAAAAA*** ~****";

            TextParser parser = new TextParser(text, 100);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(BoldStartTag));
            Assert.IsInstanceOfType(elems[1], typeof(EmphasisStartTag));
            Assert.IsInstanceOfType(elems[2], typeof(TextSpan));
            Assert.IsInstanceOfType(elems[3], typeof(EmphasisEndTag));
            Assert.IsInstanceOfType(elems[4], typeof(BoldEndTag));
            Assert.IsInstanceOfType(elems[5], typeof(TextSpan));
            Assert.AreEqual(((TextSpan)elems[5]).Text, " ~");
            Assert.IsInstanceOfType(elems[6], typeof(BoldStartTag));
            Assert.IsInstanceOfType(elems[7], typeof(BoldEndTag));
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

        [TestMethod()]
        public void ParseURLLinkTest()
        {
            string text = @"[[AAAAAAAAAA]]";

            TextParser parser = new TextParser(text, 0);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(HyperlinkSpan));
            Assert.AreEqual(((HyperlinkSpan)elems[0]).Text, "AAAAAAAAAA");
            Assert.AreEqual(((HyperlinkSpan)elems[0]).Href, "AAAAAAAAAA");
        }

        [TestMethod()]
        public void ParseLinkTest()
        {
            string text = @"[AAAAAAAAAA](BBBBBBBBBB)";

            TextParser parser = new TextParser(text, 0);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(HyperlinkSpan));
            Assert.AreEqual(((HyperlinkSpan)elems[0]).Text, "AAAAAAAAAA");
            Assert.AreEqual(((HyperlinkSpan)elems[0]).Href, "BBBBBBBBBB");
        }

        [TestMethod()]
        public void ParseImageTest()
        {
            string text = @"![AAAAAAAAAA](BBBBBBBBBB)";

            TextParser parser = new TextParser(text, 0);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(ImageSpan));
            Assert.AreEqual(((ImageSpan)elems[0]).Text, "AAAAAAAAAA");
            Assert.AreEqual(((ImageSpan)elems[0]).Source, "BBBBBBBBBB");
        }

        [TestMethod()]
        public void ParseImageTest2()
        {
            string text = @"*XXXXX*![AAAAAAAAAA](BBBBBBBBBB)[AAAAAAAAAA](BBBBBBBBBB)";

            TextParser parser = new TextParser(text, 0);
            TextElement[] elems = parser.Parse();

            Assert.IsInstanceOfType(elems[0], typeof(EmphasisStartTag));
            Assert.IsInstanceOfType(elems[1], typeof(TextSpan));
            Assert.AreEqual(((TextSpan)elems[1]).Text, "XXXXX");
            Assert.IsInstanceOfType(elems[2], typeof(EmphasisEndTag));
            Assert.IsInstanceOfType(elems[3], typeof(ImageSpan));
            Assert.AreEqual(((ImageSpan)elems[3]).Text, "AAAAAAAAAA");
            Assert.AreEqual(((ImageSpan)elems[3]).Source, "BBBBBBBBBB");
            Assert.IsInstanceOfType(elems[4], typeof(HyperlinkSpan));
            Assert.AreEqual(((HyperlinkSpan)elems[4]).Text, "AAAAAAAAAA");
            Assert.AreEqual(((HyperlinkSpan)elems[4]).Href, "BBBBBBBBBB");
        }
    }
}