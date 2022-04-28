using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    internal class PNGFile : BasicFile, IImageFile
    {
        public const uint SIGNATURE = 0x474E5089;
        private Image _image;
        public PNGFile(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null) : 
            base(stream, fileName, length, parentArchive)
        {
            using MemoryStream ms = new(GetData());
            _image = Image.FromStream(ms);
        }

        public Image Image => _image;

        public int Width => _image.Width;

        public int Height => _image.Height;

        public override string FormatName => "PNG Image";

        public override string? DefaultExtension => "PNG";

        public override void Export(string path, ExportSettings settings)
        {
            string fn = path + (settings.AddDefaultExtension ? GetFileNameWithExtension() : Name);
            Directory.CreateDirectory(Path.GetDirectoryName(fn) ?? "");
            File.WriteAllBytes(fn, GetData());
        }
    }
}
