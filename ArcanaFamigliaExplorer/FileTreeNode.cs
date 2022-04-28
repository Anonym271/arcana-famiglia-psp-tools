using LibArcanaFamiglia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcanaFamigliaExplorer
{
    internal interface IFileTreeNode
    {
        public BasicFile File { get; }
    }

    internal class FileTreeNode : TreeNode, IFileTreeNode
    {
        private readonly BasicFile _file;
        public BasicFile File => _file;

        public FileTreeNode(BasicFile file)
        {
            _file = file;
            Text = $"{file.Name} [{file.FormatName}]";

            if (file is IArchive archive)
            {
                foreach (var entry in archive.Files)
                    Nodes.Add(new FileTreeNode(entry));
            }
        }
    }
    
    internal class LazyFileTreeNode : TreeNode, IFileTreeNode
    {
        private bool _expanded = false;
        private readonly BasicFile _file;
        public BasicFile File => _file;

        public LazyFileTreeNode(BasicFile file)
        {
            _file = file;
            Text = $"{file.Name} [{file.FormatName}]";

            if (file is IArchive)
                Nodes.Add("Loading...");
        }

        public void ExpandArchive()
        {
            if (!_expanded && _file is IArchive archive)
            {
                TreeNode[] nodes = new TreeNode[archive.Count];
                int i = 0;
                foreach (var entry in archive.Files)
                {
                    nodes[i] = new LazyFileTreeNode(entry);
                    i++;
                }
                Nodes.Clear();
                Nodes.AddRange(nodes);
            }
            _expanded = true;
        }
    }
}
