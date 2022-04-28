using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    internal record HFU2EntryHeader
    {
        public readonly int Offset;
        public readonly int ContentLength;
        public readonly int UncompressedLength;
        public readonly int Unknown;

        public HFU2EntryHeader(BinaryReader stream)
        {
            Offset = stream.ReadInt32();
            ContentLength = stream.ReadInt32();
            UncompressedLength = stream.ReadInt32();
            Unknown = stream.ReadInt32();
        }
    }

    internal class HFU2Entry : IArchiveEntry
    {
        private BasicFile? _file;
        private HFU2Archive _parentArchive;
        private BinaryReader _in;
        private HFU2EntryHeader _hdr;
        private long _baseOffset;
        private int _index;

        public IArchive ParentArchive => _parentArchive;
        public BasicFile File
        {
            get
            {
                if (_file == null)
                {
                    var fileStream = DecompressFile();
                    _file = FileFactory.CreateFile(fileStream, _index.ToString(), _hdr.UncompressedLength, _parentArchive);
                }
                return _file;
            }
        }
        public long Length => _hdr.ContentLength;

        private Stream DecompressFile()
        {
            _in.BaseStream.Position = _baseOffset + _hdr.Offset;

            if (!_parentArchive.IsCompressed)
                return _in.BaseStream;
     
            uint uffaSignature = _in.ReadUInt32();
            if (uffaSignature != 0x41464655) // "UFFA"
                throw new SignatureException("UFFA Compression");
            int sizeCompressed = _in.ReadInt32();
            int sizeResult = _in.ReadInt32();
            _in.BaseStream.Position += 4;

            byte[] data = _in.ReadBytes(sizeCompressed);
            return new MemoryStream(LZSS.Decompress(data, sizeResult));
        }

        public HFU2Entry(HFU2Archive archive, BinaryReader inputStream, HFU2EntryHeader header, long baseOffset, int index)
        {
            _parentArchive = archive;
            _in = inputStream;
            _hdr = header;
            _baseOffset = baseOffset;
            _index = index;
        }
    }

    internal class HFU2Archive : BasicFile, IArchive
    {
        public const uint SIGNATURE = 0x32554648;

        private HFU2Entry[] _entries;
        private long _baseOffset;
        private int _contentOffset;

        public readonly bool IsCompressed;

        public IEnumerable<IArchiveEntry> Entries => _entries.AsEnumerable();
        public int Count => _entries.Length;
        public override string FormatName => "HFU2 Archive";
        public override string DefaultExtension => "HFU";

        public HFU2Archive(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null) :
            base(stream, fileName, length, parentArchive)
        {
            _baseOffset = stream.Position;

            uint signature = _in.ReadUInt32();
            if (signature != SIGNATURE)
                throw new SignatureException("HFU2", SIGNATURE, signature);

            int fileCount = _in.ReadInt32();
            _contentOffset = _in.ReadInt32();
            int compressed = _in.ReadInt32();
            IsCompressed = compressed != 0;

            var entryHeaders = new HFU2EntryHeader[fileCount];
            for (int i = 0; i < fileCount; i++)
                entryHeaders[i] = new(_in);

            _entries = new HFU2Entry[fileCount];
            for (int i = 0; i < fileCount; i++)
                _entries[i] = new(this, _in, entryHeaders[i], _baseOffset + _contentOffset, i);
        }

        public override void Export(string path, ExportSettings settings)
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
    }
}
