using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdoc
{
    public class HtmlEncodeParameters
    {
        private bool headerIncludes = true;

        public bool HeaderIncludes
        {
            get { return headerIncludes; }
            set { headerIncludes = value; }
        }

        public string DocumentTitle
        {
            get; set;
        }

        public Uri CssUrl
        {
            get; set;
        }
    }
}
