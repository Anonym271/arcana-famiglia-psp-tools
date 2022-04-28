using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace LibArcanaFamiglia
{
    internal class AFSEntry : IArchiveEntry
    {
        private readonly AFSArchive _parentArchive;
        private readonly BinaryReader _in;
        private readonly long _baseOffset;
        private readonly int _offset;
        private readonly int _length;
        private readonly AFSEntryDescriptor _descriptor;

        private BasicFile? _file;

        public BasicFile File
        {
            get
            {
                if (_file == null)
                {
                    var stream = _in.BaseStream;
                    stream.Position = _baseOffset + _offset;
                    _file = FileFactory.CreateFile(stream, _descriptor.Name, _length, _parentArchive);
                }
                return _file;
            }
        }
        public IArchive ParentArchive => _parentArchive;
        public long Length => _length;


        public AFSEntry(AFSArchive archive, BinaryReader inputStream, AFSTableEntry tableEntry, AFSEntryDescriptor descriptor, long baseOffset = 0)
        {
            _parentArchive = archive;
            _in = inputStream;
            _baseOffset = baseOffset;
            _offset = tableEntry.Offset;
            _length = tableEntry.Length;
            _descriptor = descriptor;
        }
    }

    internal record AFSEntryDescriptor
    {
        private static Encoding? _encoding;
        public static Encoding TextEncoding
        {
            get
            {
                if (_encoding == null)
                {
                    _encoding = CodePagesEncodingProvider.Instance.GetEncoding(932);
                    if (_encoding == null)
                        _encoding = Encoding.ASCII;
                }
                return _encoding;
            }
            set => _encoding = value;
        }

        public string Name;
        public readonly ushort Year;
        public readonly ushort Month;
        public readonly ushort Day;
        public readonly ushort Hour;
        public readonly ushort Minute;
        public readonly ushort Second;
        public readonly uint FileLength;

        public AFSEntryDescriptor(BinaryReader file)
        {
            Name = TextEncoding.GetString(file.ReadBytes(32)).TrimEnd('\0');
            Year = file.ReadUInt16();
            Month = file.ReadUInt16();
            Day = file.ReadUInt16();
            Hour = file.ReadUInt16();
            Minute = file.ReadUInt16();
            Second = file.ReadUInt16();
            FileLength = file.ReadUInt32();
        }
    }

    internal record AFSTableEntry(int Offset, int Length);

    internal class AFSArchive : BasicFile, IArchive
    {
        public const uint SIGNATURE = 0x534641;
        
        private List<AFSEntry> _entries;

        public IEnumerable<IArchiveEntry> Entries => _entries;
        public int Count => _entries.Count;
        public override string FormatName => "AFS Archive";
        public override string DefaultExtension => "AFS";

        public AFSArchive(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null) : 
            base(stream, fileName, length, parentArchive)
        {
            uint sig = _in.ReadUInt32();
            if (sig != SIGNATURE)
                throw new SignatureException("AFS", SIGNATURE, sig);
            
            int fileCount = _in.ReadInt32();

            var entryTable = new List<AFSTableEntry>(fileCount);
            for (int i = 0; i < fileCount; i++)
                entryTable.Add(new(_in.ReadInt32(), _in.ReadInt32()));
            AFSTableEntry descriptorEntry = new(_in.ReadInt32(), _in.ReadInt32());

            Seek(descriptorEntry.Offset);
            var descriptors = new List<AFSEntryDescriptor>(fileCount);
            var names = new Dictionary<string, int>();
            for (int i = 0; i < fileCount; i++)
            {
                var desc = new AFSEntryDescriptor(_in);
                if (string.IsNullOrWhiteSpace(desc.Name))
                    desc.Name = i.ToString();
                if (!names.TryGetValue(desc.Name, out int nameCount))
                    nameCount = -1;
                nameCount++;
                names[desc.Name] = nameCount;
                if (nameCount > 0)
                    desc.Name += $" ({nameCount})";
                descriptors.Add(desc);
            }

            _entries = new(fileCount);
            for (int i = 0; i < fileCount; i++)
                _entries.Add(new(this, _in, entryTable[i], descriptors[i]));
        }

        public override void Export(string path, ExportSettings settings)
        {
            var name = Path.GetFileNameWithoutExtension(Name);
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
            path += name + Path.DirectorySeparatorChar;
            //Directory.CreateDirectory(path);
            foreach (var e in _entries)
                e.File.Export(path, settings);
        }
    }
}
