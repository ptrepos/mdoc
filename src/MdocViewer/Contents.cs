using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace MdocViewer
{
    public class Contents
    {
        public static Uri GetCssPath()
        {
            return new Uri("style/doc.css", UriKind.Relative);
        }
        public static Uri GetCssAbsolutePath()
        {
            return new Uri("file:///" + Path.Combine(Application.StartupPath, "style/doc.css"), UriKind.Absolute);
        }
    }
}
