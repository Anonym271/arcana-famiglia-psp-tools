using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    internal enum TTHPaletteType
    {
        P16 = 2,
        P256 = 4,
    }


    internal class TTHEntry : IArchiveEntry
    {
        public const int SIZE = 16;

        public readonly uint unk_00;
        public readonly uint Offset;
        public readonly TTHPaletteType PaletteType;
        public readonly int Width;
        public readonly int Height;
        public readonly int Index;
        public readonly TTHImageArchive TTHArchive;
        public readonly long BaseOffset;

        private readonly BinaryReader _in;
        private TTHImageFile? _file;

        public int PaletteColorCount => PaletteType switch
        {
            TTHPaletteType.P16 => 16,
            TTHPaletteType.P256 => 256,
            _ => throw new InvalidOperationException($"Unknown palette type: {PaletteType}")
        };
        public int PaletteSize => PaletteColorCount * 4;
        public int ImageDataSize => (Width * Height * BPP) / 8;
        public int BPP => PaletteType switch
        {
            TTHPaletteType.P16 => 4,
            TTHPaletteType.P256 => 8,
            _ => throw new InvalidOperationException($"Unknown palette type: {PaletteType}")
        };
        public long Length => PaletteSize + ImageDataSize;
        public BasicFile File { get
            {
                if (_file == null)
                {
                    _in.BaseStream.Position = BaseOffset + Offset;
                    _file = new TTHImageFile(this);
                }
                return _file;
            } }
        public IArchive ParentArchive => TTHArchive;

        public TTHEntry(TTHImageArchive archive, int index)
        {
            TTHArchive = archive;
            _in = archive.BinaryReader;
            Index = index;
            BaseOffset = archive.ContentOffset;

            unk_00 = _in.ReadUInt32();
            Offset = _in.ReadUInt32();
            PaletteType = (TTHPaletteType)_in.ReadInt32();
            Width = _in.ReadUInt16();
            //if (PaletteType == TTHPaletteType.P16)
            //    Width /= 2;
            Height = _in.ReadUInt16();
        }

    }

    internal class TTHImageArchive : BasicFile, IArchive
    {
        public const uint SIGNATURE = 0x485454;

        private readonly long _baseOffset;
        private readonly uint _unk_04;
        private readonly uint _unk_0C;
        private readonly TTHEntry[] _entries;
        public readonly long ContentOffset;

        public IEnumerable<IArchiveEntry> Entries => _entries.AsEnumerable();
        public int Count => _entries.Length;

        public TTHImageArchive(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null) : 
            base(stream, fileName, length, parentArchive)
        {
            _baseOffset = stream.Position;

            uint signature = _in.ReadUInt32();
            if (signature != SIGNATURE)
                throw new SignatureException("TTH", SIGNATURE, signature);

            _unk_04 = _in.ReadUInt32();
            uint count = _in.ReadUInt32();
            _unk_0C = _in.ReadUInt32();

            ContentOffset = stream.Position + count * TTHEntry.SIZE + 16;

            _entries = new TTHEntry[count];
            for (int i = 0; i < count; i++)
                _entries[i] = new(this, i);
            
            // Ignore unknown additional header (for now)
        }

        public BinaryReader BinaryReader => _in;

        public override string FormatName => "TTH Image Archive";

        public override string? DefaultExtension => "TTH";

        public override void Export(string path, ExportSettings settings)
        {
            // Extract single image files only if ConvertImages is enabled
            if (settings.ConvertImages) 
            {
                if (settings.ReduceFolders)
                {
                    if (Count == 0)
                        return;
                    if (Count == 1)
                    {
                        _entries[0].File.Export($"{path}{Name}_", settings);
                        return;
                    }
                }
                path += Name + Path.DirectorySeparatorChar;
                //Directory.CreateDirectory(path);
                foreach (var e in _entries)
                    e.File.Export(path , settings);
            }
            else
            {
                File.WriteAllBytes(path + (settings.AddDefaultExtension ? GetFileNameWithExtension() : Name), GetData());
            }
        }
    }
}
