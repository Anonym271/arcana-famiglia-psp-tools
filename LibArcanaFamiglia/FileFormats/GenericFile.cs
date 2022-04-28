using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    internal class GenericFile : BasicFile
    {
        public GenericFile(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null) : 
            base(stream, fileName, length, parentArchive)
        {
        }

        public override string FormatName => "Unknown";

        public override string? DefaultExtension => null;

        public override void Export(string path, ExportSettings settings)
        {
            string fn = path + (settings.AddDefaultExtension ? GetFileNameWithExtension() : Name);
            Directory.CreateDirectory(Path.GetDirectoryName(fn) ?? "");
            File.WriteAllBytes(fn, GetData());
        }
    }
}
