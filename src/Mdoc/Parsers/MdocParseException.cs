using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdoc.Parsers
{
    public class MdocParseException : Exception
    {
        private int line;

        public MdocParseException(string message, int line) : base(string.Format("[line:{0}] ", line) + message)
        {
            this.line = line;
        }

        public int Line
        {
            get { return line; }
        }
    }
}
