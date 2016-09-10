using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mdoc;

namespace MdocViewer
{
    public partial class MdocViewerForm : Form
    {
        private string mdocFile;

        public MdocViewerForm()
        {
            InitializeComponent();
        }

        public string DocumentText
        {
            get { return browser.DocumentText; }
            set { browser.DocumentText = value; }
        }

        private void MdocViewerForm_Load(object sender, EventArgs e)
        {
            SetFormMode();
        }

        private void MdocViewerForm_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetFormats();

            object obj = e.Data.GetData("FileNameW");
            if (obj == null)
                return;

            string[] fileNames = ((string[])obj);
            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(Mdoc.MdocTool.FILE_EXTENSION))
                {
                    e.Effect = DragDropEffects.Copy;
                    break;
                }
            }
        }

        private void MdocViewerForm_DragDrop(object sender, DragEventArgs e)
        {
            object obj = e.Data.GetData("FileNameW");
            if (obj == null)
                return;

            string[] fileNames = ((string[])obj);
            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(Mdoc.MdocTool.FILE_EXTENSION))
                {
                    LoadMdocFile(fileName);
                    break;
                }
            }
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.Text = browser.DocumentTitle;

            SetFormMode();
        }

        private void fileOpenItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "mdoc|*.mdoc|All Files|*.*";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadMdocFile(dialog.FileName);
            }
        }

        private void endItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
        }

        private void updateButon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(mdocFile))
            {
                return;
            }

            LoadMdocFile(mdocFile);
        }

        public void LoadMdocFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                StringWriter writer = new StringWriter();

                MdocTool.EncodeHtml(
                    writer,
                    reader,
                    Path.GetFileNameWithoutExtension(fileName),
                    Contents.GetCssPath());

                mdocFile = fileName;
                browser.DocumentText = writer.ToString();
            }
        }

        private void SetFormMode()
        {
            if (string.IsNullOrEmpty(mdocFile))
            {
                updateButton.Enabled = false;
                browser.Visible = true;
            }
            else
            {
                updateButton.Enabled = true;
                browser.Visible = true;
            }
        }
    }
}
