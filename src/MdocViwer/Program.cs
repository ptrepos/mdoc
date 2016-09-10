using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MdocViewer
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MdocViewerForm form = new MdocViewerForm();
            if (args.Length > 0)
            {
                form.LoadMdocFile(args[0]);
            }

            Application.Run(form);
        }
    }
}
