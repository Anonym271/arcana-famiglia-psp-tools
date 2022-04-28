using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LibArcanaFamiglia
{
    internal class TTHImageFile : BasicFile, IImageFile
    {
        private const int TILE_WIDTH = 16;
        private const int TILE_HEIGHT = 8;

        private readonly TTHEntry _entry;
        private readonly Bitmap _bmp;

        public Image Image => _bmp;
        public int Width => _entry.Width;
        public int Height => _entry.Height;

        public override string FormatName => "TTH Image";

        public override string? DefaultExtension => null;

        public TTHImageFile(TTHEntry entry) : 
            base(entry.TTHArchive.BinaryReader.BaseStream, entry.Index.ToString(), entry.Length, entry.TTHArchive)
        {
            _entry = entry;
            if (Width == 0 || Height == 0)
            {
                _bmp = CreatePaletteBitmap();
                return;
            }

            _bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

            var imgData = AssembleTiles(GetImageData());
            if (entry.BPP == 4)
                imgData = Extend4BPPTo8BPP(imgData);
            else if (entry.BPP != 8)
                throw new InvalidOperationException($"Unknown BPP: {entry.BPP}");
            var bitmapData = _bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            Marshal.Copy(imgData, 0, bitmapData.Scan0, imgData.Length);
            _bmp.UnlockBits(bitmapData);

            var pal = _bmp.Palette;
            var paletteData = GetPaletteData();
            int j;
            for (int i = 0; i < entry.PaletteColorCount; i++)
            {
                j = i * 4;
                pal.Entries[i] = Color.FromArgb(paletteData[j + 3], paletteData[j], paletteData[j + 1], paletteData[j + 2]);
            }
            _bmp.Palette = pal;
        }

        private Bitmap CreatePaletteBitmap()
        {
            int size = _entry.PaletteType switch
            {
                TTHPaletteType.P16 => 4,
                TTHPaletteType.P256 => 16,
                _ => throw new InvalidOperationException($"Unknown palette type: {_entry.PaletteType}")
            };
            byte[] pal = GetPaletteData();
            byte[] newpal = new byte[pal.Length];
            for (int i = 0; i < pal.Length; i += 4)
            {
                newpal[i + 0] = pal[i + 3];
                newpal[i + 1] = pal[i + 0];
                newpal[i + 2] = pal[i + 1];
                newpal[i + 3] = pal[i + 2];
            }
            
            Bitmap bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(newpal, 0, data.Scan0, newpal.Length);
            bmp.UnlockBits(data);
            return bmp;
        }

        private static byte[] Extend4BPPTo8BPP(byte[] input)
        {
            byte[] res = new byte[input.Length * 2];
            for (int i = 0; i < input.Length; i++)
            {
                res[i * 2 + 0] = (byte)(input[i] & 0x0F);
                res[i * 2 + 1] = (byte)((input[i] & 0xF0) >> 4);
            }
            return res;
        }

        private byte[] AssembleTiles(byte[] data)
        {
            int w = (_entry.Width * _entry.BPP) / 8;
            int h = _entry.Height;
            byte[] result = new byte[data.Length];

            int cols = w / TILE_WIDTH;
            int tiled_width = cols * TILE_WIDTH;
            int x_overflow = w - tiled_width;
            int rows = h / TILE_HEIGHT;
            int tiled_height = rows * TILE_HEIGHT;
            int y_overflow = h - tiled_height;

            Debug.Assert(x_overflow == 0); // Hope this never happens...

            int stride = cols * TILE_WIDTH;
            int tileStride = stride * TILE_HEIGHT;
            int pos = 0;
            int col = 0;
            int row = 0;
            while (pos < data.Length && row < rows)
            {
                for (int i = 0; i < TILE_HEIGHT; i++)
                {
                    Array.Copy(data, pos, result, row * tileStride + i * stride + col * TILE_WIDTH, TILE_WIDTH);
                    pos += TILE_WIDTH;
                }
                if (++col >= cols)
                {
                    col = 0;
                    row++;
                }
            }

            // Add remaining partial row, if necessary
            if (y_overflow > 0)
            {
                for (col = 0; col < cols; col++)
                {
                    for (int i = 0; i < y_overflow; i++)
                    {
                        Array.Copy(data, pos, result, row * tileStride + i * stride + col * TILE_WIDTH, TILE_WIDTH);
                        pos += TILE_WIDTH;
                    }
                }
            }

            return result;
        }

        public byte[] GetPaletteData()
        {
            _stream.Position = _entry.BaseOffset + _entry.Offset;
            return _in.ReadBytes(_entry.PaletteSize);
        }

        public byte[] GetImageData()
        {
            _stream.Position = _entry.BaseOffset + _entry.Offset + _entry.PaletteSize;
            return _in.ReadBytes(_entry.ImageDataSize);
        }

        public override void Export(string path, ExportSettings settings)
        {
            if (settings.ConvertImages)
            {
                string fn = path + Name + ".PNG";
                Directory.CreateDirectory(Path.GetDirectoryName(fn) ?? "");
                Image.Save(fn);
            }
            else
            {
                string fn = path + (settings.AddDefaultExtension ? GetFileNameWithExtension() : Name);
                Directory.CreateDirectory(Path.GetDirectoryName(fn) ?? "");
                File.WriteAllBytes(fn, GetData());
            }
        }
    }
}
