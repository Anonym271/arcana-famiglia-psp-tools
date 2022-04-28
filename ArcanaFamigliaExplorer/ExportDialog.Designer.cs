namespace ArcanaFamigliaExplorer
{
    partial class ExportDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.cbAddExtensions = new System.Windows.Forms.CheckBox();
            this.cbConvertImages = new System.Windows.Forms.CheckBox();
            this.cbCollapseFolders = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // tbFileName
            // 
            this.tbFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFileName.Location = new System.Drawing.Point(12, 12);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(448, 23);
            this.tbFileName.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(466, 11);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // cbAddExtensions
            // 
            this.cbAddExtensions.AutoSize = true;
            this.cbAddExtensions.Checked = true;
            this.cbAddExtensions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAddExtensions.Location = new System.Drawing.Point(12, 41);
            this.cbAddExtensions.Name = "cbAddExtensions";
            this.cbAddExtensions.Size = new System.Drawing.Size(126, 19);
            this.cbAddExtensions.TabIndex = 2;
            this.cbAddExtensions.Text = "Add file extensions";
            this.cbAddExtensions.UseVisualStyleBackColor = true;
            // 
            // cbConvertImages
            // 
            this.cbConvertImages.AutoSize = true;
            this.cbConvertImages.Checked = true;
            this.cbConvertImages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbConvertImages.Location = new System.Drawing.Point(12, 66);
            this.cbConvertImages.Name = "cbConvertImages";
            this.cbConvertImages.Size = new System.Drawing.Size(150, 19);
            this.cbConvertImages.TabIndex = 2;
            this.cbConvertImages.Text = "Convert Images to PNG";
            this.cbConvertImages.UseVisualStyleBackColor = true;
            // 
            // cbCollapseFolders
            // 
            this.cbCollapseFolders.AutoSize = true;
            this.cbCollapseFolders.Location = new System.Drawing.Point(12, 91);
            this.cbCollapseFolders.Name = "cbCollapseFolders";
            this.cbCollapseFolders.Size = new System.Drawing.Size(246, 19);
            this.cbCollapseFolders.TabIndex = 2;
            this.cbCollapseFolders.Text = "Collapse folders with less than two entries";
            this.cbCollapseFolders.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(466, 145);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(385, 145);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 116);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(529, 23);
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;
            // 
            // ExportDialog
            // 
            this.AcceptButton = this.btnExport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 180);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.cbCollapseFolders);
            this.Controls.Add(this.cbConvertImages);
            this.Controls.Add(this.cbAddExtensions);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ExportDialog";
            this.Text = "Export";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbFileName;
        private Button btnBrowse;
        private CheckBox cbAddExtensions;
        private CheckBox cbConvertImages;
        private CheckBox cbCollapseFolders;
        private Button btnExport;
        private Button btnCancel;
        private ProgressBar progressBar;
    }
}