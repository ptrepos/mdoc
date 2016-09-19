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
        private const string OPEN_FILTER = "mdoc|*.mdoc|All Files|*.*";
        private const string SAVE_FILTER = "HTML|*.html|All Files|*.*";

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
            this.Text = "Mdoc Viewer [" + browser.DocumentTitle + "]";

            SetFormMode();
        }

        private void fileOpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = OPEN_FILTER;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadMdocFile(dialog.FileName);
            }
        }

        private void fileSaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = SAVE_FILTER;
            dialog.FileName = Path.GetFileNameWithoutExtension(mdocFile) + ".html";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveHtmlFile(dialog.FileName, mdocFile);
            }
        }

        private void fileSaveWithoutHeaderMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = SAVE_FILTER;
            dialog.FileName = Path.GetFileNameWithoutExtension(mdocFile) + ".html";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveHtmlFile(dialog.FileName, mdocFile, false);
            }
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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
                StringWriter messageWriter = new StringWriter();

                HtmlEncodeParameters parameters = new HtmlEncodeParameters();
                parameters.HeaderIncludes = true;
                parameters.DocumentTitle = Path.GetFileNameWithoutExtension(fileName);
                parameters.CssUrl = Contents.GetCssAbsolutePath();

                MdocTool.EncodeHtml(
                    writer,
                    reader,
                    messageWriter,
                    parameters);
                
                mdocFile = fileName;
                browser.DocumentText = writer.ToString();

                messageBox.Text = messageWriter.ToString();
            }
        }

        public void SaveHtmlFile(string saveFile, string fileName, bool headerIncludes = true)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                using (StreamWriter writer = new StreamWriter(saveFile, false, Encoding.UTF8))
                {
                    StringWriter messageWriter = new StringWriter();

                    HtmlEncodeParameters parameters = new HtmlEncodeParameters();
                    parameters.HeaderIncludes = headerIncludes;
                    parameters.DocumentTitle = Path.GetFileNameWithoutExtension(fileName);
                    parameters.CssUrl = Contents.GetCssPath();

                    MdocTool.EncodeHtml(
                        writer,
                        reader,
                        messageWriter,
                        parameters);
                }
            }
        }

        private void SetFormMode()
        {
            if (string.IsNullOrEmpty(mdocFile))
            {
                updateButton.Enabled = false;
                browser.Visible = true;
                fileSaveMenuItem.Enabled = false;
                fileSaveWithoutHeaderMenuItem.Enabled = false;
            }
            else
            {
                updateButton.Enabled = true;
                browser.Visible = true;
                fileSaveMenuItem.Enabled = true;
                fileSaveWithoutHeaderMenuItem.Enabled = true;
            }
        }

        private void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.LocalPath.EndsWith(".mdoc"))
            {
                LoadMdocFile(Path.Combine(Path.GetDirectoryName(mdocFile), e.Url.LocalPath));
                e.Cancel = true;
            }
            if (!e.Url.LocalPath.Contains("."))
            {
                string path = Path.Combine(Path.GetDirectoryName(mdocFile), e.Url.LocalPath + ".mdoc");
                if (File.Exists(path))
                {
                    LoadMdocFile(path);
                    e.Cancel = true;
                }
            }
        }
    }
}
