namespace MdocViewer
{
    partial class MdocViewerForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.browser = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveWithoutHeaderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateButton = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // browser
            // 
            this.browser.AllowWebBrowserDrop = false;
            this.browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.browser.Location = new System.Drawing.Point(0, 0);
            this.browser.Margin = new System.Windows.Forms.Padding(0);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(1008, 425);
            this.browser.TabIndex = 0;
            this.browser.Tag = "";
            this.browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.browser_DocumentCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileItem,
            this.helpItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 26);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileItem
            // 
            this.fileItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenMenuItem,
            this.toolStripSeparator2,
            this.fileSaveMenuItem,
            this.fileSaveWithoutHeaderMenuItem,
            this.toolStripSeparator1,
            this.closeMenuItem});
            this.fileItem.Name = "fileItem";
            this.fileItem.Size = new System.Drawing.Size(68, 22);
            this.fileItem.Text = "ファイル";
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Name = "fileOpenMenuItem";
            this.fileOpenMenuItem.Size = new System.Drawing.Size(264, 22);
            this.fileOpenMenuItem.Text = "ファイルを開く";
            this.fileOpenMenuItem.Click += new System.EventHandler(this.fileOpenMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(261, 6);
            // 
            // fileSaveMenuItem
            // 
            this.fileSaveMenuItem.Name = "fileSaveMenuItem";
            this.fileSaveMenuItem.Size = new System.Drawing.Size(264, 22);
            this.fileSaveMenuItem.Text = "ファイルを保存";
            this.fileSaveMenuItem.Click += new System.EventHandler(this.fileSaveMenuItem_Click);
            // 
            // fileSaveWithoutHeaderMenuItem
            // 
            this.fileSaveWithoutHeaderMenuItem.Name = "fileSaveWithoutHeaderMenuItem";
            this.fileSaveWithoutHeaderMenuItem.Size = new System.Drawing.Size(264, 22);
            this.fileSaveWithoutHeaderMenuItem.Text = "ファイルを保存(HTMLヘッダ除く)";
            this.fileSaveWithoutHeaderMenuItem.Click += new System.EventHandler(this.fileSaveWithoutHeaderMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(261, 6);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(264, 22);
            this.closeMenuItem.Text = "プログラムを終了";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // helpItem
            // 
            this.helpItem.Name = "helpItem";
            this.helpItem.Size = new System.Drawing.Size(56, 22);
            this.helpItem.Text = "ヘルプ";
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateButton.Location = new System.Drawing.Point(954, 0);
            this.updateButton.Margin = new System.Windows.Forms.Padding(0);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(54, 24);
            this.updateButton.TabIndex = 3;
            this.updateButton.Text = "更新";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButon_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 26);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.browser);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.messageBox);
            this.splitContainer.Size = new System.Drawing.Size(1008, 535);
            this.splitContainer.SplitterDistance = 425;
            this.splitContainer.TabIndex = 4;
            // 
            // messageBox
            // 
            this.messageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageBox.BackColor = System.Drawing.SystemColors.Window;
            this.messageBox.Location = new System.Drawing.Point(0, 0);
            this.messageBox.Margin = new System.Windows.Forms.Padding(0);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.ReadOnly = true;
            this.messageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageBox.Size = new System.Drawing.Size(1008, 106);
            this.messageBox.TabIndex = 0;
            // 
            // MdocViewerForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MdocViewerForm";
            this.Text = "Mdoc Viewer";
            this.Load += new System.EventHandler(this.MdocViewerForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MdocViewerForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MdocViewerForm_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem helpItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.ToolStripMenuItem fileSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveWithoutHeaderMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

