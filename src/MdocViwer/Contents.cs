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
        public static string GetCssPath()
        {
            return Path.Combine(Application.StartupPath, "style/doc.css");
        }
    }
}
