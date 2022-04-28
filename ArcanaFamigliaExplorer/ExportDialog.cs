using LibArcanaFamiglia;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcanaFamigliaExplorer
{
    public partial class ExportDialog : Form
    {
        //public ExportSettings ExportSettings;
        private BasicFile _file;
        private ExportSettings _settings = new();
        Thread? _exportThread;

        public ExportDialog(string defaultFilename, BasicFile file)
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            //ExportSettings = new(defaultFilename);
            tbFileName.Text = defaultFilename;
            _file = file;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using CommonOpenFileDialog ofd = new("Export Location");
           // if (_file is IArchive arc)
                ofd.IsFolderPicker = true;
            ofd.DefaultDirectory = Path.GetDirectoryName(tbFileName.Text);
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                tbFileName.Text = ofd.FileName;
        }

        private void btnCancel_Click(object sender, EventArgs e) => Close();

        private void btnExport_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            tbFileName.Enabled = false;
            btnBrowse.Enabled = false;
            btnExport.Enabled = false;
            btnCancel.Enabled = false;
            cbAddExtensions.Enabled = false;
            cbCollapseFolders.Enabled = false;
            cbConvertImages.Enabled = false;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;

            string path = tbFileName.Text;
            if (!Path.EndsInDirectorySeparator(path))
                path += Path.DirectorySeparatorChar;

            _settings = new ExportSettings(path)
            {
                AddDefaultExtension = cbAddExtensions.Checked,
                ConvertImages = cbConvertImages.Checked,
                ReduceFolders = cbCollapseFolders.Checked,
            };
            //_exportThread = new(Export);
            //_exportThread.Start();
            
            _file.Export(_settings.RootPath, _settings);
            Close();
        }

        private void Export()
        {
            try
            {
                _file.Export(_settings.RootPath, _settings);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Close();
        }
    }
}
