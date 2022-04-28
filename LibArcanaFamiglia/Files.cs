using System.Collections;
using System.Drawing;

namespace LibArcanaFamiglia
{

    public abstract class BasicFile
    {
        protected Stream _stream;
        protected BinaryReader _in;
        protected long _streamOffset;
        protected string _fileName;
        protected long _length;
        protected IArchive? _parentArchive;

        public BasicFile(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null)
        {
            _stream = stream;
            _streamOffset = stream.Position;
            _in = new BinaryReader(_stream, System.Text.Encoding.Default, true);
            _fileName = fileName;
            _length = length >= 0 ? length : stream.Length - stream.Position;
            _parentArchive = parentArchive;
        }

        public virtual string Name => _fileName;
        public abstract string FormatName { get; }// => "Unknown";
        public abstract string? DefaultExtension { get; }// => null;
        public virtual long Length => _length;
        public virtual long Size => _length;
        public virtual IArchive? ParentArchive => _parentArchive;

        public virtual byte[] GetData()
        {
            _stream.Position = _streamOffset;
            return _in.ReadBytes((int)_length);
        }

        public virtual Stream GetStream()
        {
            _stream.Position = _streamOffset;
            return _stream;
        }

        public virtual string GetFileNameWithExtension()
        {
            if (DefaultExtension == null)
                return Name;
            if (Name.ToLower().EndsWith(DefaultExtension.ToLower()))
                return Name;
            return $"{Name}.{DefaultExtension}";
        }

        protected virtual void Seek(long offset) { _stream.Position = _streamOffset + offset; }

        public abstract void Export(string path, ExportSettings settings);
    }

    public interface IArchiveEntry
    {
        public BasicFile File { get; }
        public IArchive ParentArchive { get; }
        public long Length { get; }

    }

    public interface IArchive : IEnumerable<IArchiveEntry>
    {
        public IEnumerable<IArchiveEntry> Entries { get; }
        public IEnumerable<BasicFile> Files { get { foreach (var e in Entries) yield return e.File; } }
        public int Count { get; }
        public string Name { get; }
        IEnumerator<IArchiveEntry> IEnumerable<IArchiveEntry>.GetEnumerator() => Entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
    }

    public interface IImageFile
    {
        public Image Image { get; }
        public int Width { get; }
        public int Height { get; }
    }
}