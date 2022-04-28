using LibArcanaFamiglia;
using Be.Windows.Forms;
using System.Runtime.InteropServices;

namespace ArcanaFamigliaExplorer
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;


        private string? currentFileName;
        private BasicFile? currentFile;

        private readonly TabPage imageTabPage = new("Image");
        private readonly PictureBox imageBox = new();

        public Form1()
        {
            InitializeComponent();

            imageBox.Dock = DockStyle.Fill;
            imageBox.BackColor = Color.Black;
            imageBox.BackgroundImageLayout = ImageLayout.Tile;
            Bitmap transp = new(16, 16);
            using (Graphics gfx = Graphics.FromImage(transp))
            {
                gfx.FillRectangle(Brushes.LightGray, 0, 0, 16, 16);
                gfx.FillRectangle(Brushes.Gray,  0, 0, 8, 8);
                gfx.FillRectangle(Brushes.Gray,  8, 8, 16, 16);
            }
            imageBox.BackgroundImage = transp;
            imageTabPage.Controls.Add(imageBox);
        }

        public void OpenFile(string filename)
        {
            var name = Path.GetFileName(filename);
            try
            {
                var fileStream = File.OpenRead(filename);
                var file = FileFactory.CreateFile(fileStream, name);

                if (file is not IArchive)
                    MessageBox.Show("Warning: The file could not be recognized as an archive.", "Warning", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                currentFile = file;
                currentFileName = filename;

                ResetContentTree();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Could not open {name}: {exc.Message}\n\n{exc.StackTrace}", "Failed", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetContentTree()
        {
            contentTreeView.Nodes.Clear();
            if (currentFile == null)
                return;
            contentTreeView.Nodes.Add(new LazyFileTreeNode(currentFile));
            contentTreeView.Nodes[0].Expand();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                OpenFile(ofd.FileName);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (contentTreeView.SelectedNode is IFileTreeNode node)
            {
                var file = node.File;
                string fn = Path.GetDirectoryName(currentFileName) ?? Environment.CurrentDirectory;
                var ed = new ExportDialog(fn, file);
                ed.ShowDialog();
                //if (file is IArchive)
                //    MessageBox.Show("Archive Export is not yet supported");
                //else
                //{
                //    using SaveFileDialog sfd = new();
                //    if (sfd.ShowDialog() == DialogResult.OK)
                //        File.WriteAllBytes(sfd.FileName, file.GetData());
                //}
            }
        }

        private void contentTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is LazyFileTreeNode node)
                node.ExpandArchive();
        }

        private void contentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SendMessage(Handle, WM_SETREDRAW, false, 0);
            tabView.TabPages.Clear();
            if (e.Node is IFileTreeNode node)
            {
                hexBox.ByteProvider = new DynamicByteProvider(node.File.GetData());
                tabView.TabPages.Add(hexTabPage);
                if (node.File is IImageFile image)
                {
                    imageBox.Image = image.Image;
                    var index = tabView.TabCount;
                    tabView.TabPages.Add(imageTabPage);
                    tabView.SelectedIndex = index;
                }
            }
            ActiveControl = contentTreeView;
            SendMessage(Handle, WM_SETREDRAW, true, 0);
            Refresh();
        }

    }
}