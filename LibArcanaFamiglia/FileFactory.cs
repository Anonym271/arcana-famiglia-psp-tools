using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    public static class FileFactory
    {
        public static BasicFile CreateFile(Stream stream, string fileName, long length = -1, IArchive? parentArchive = null)
        {
            if (0 <= length && length < 4)
                return new GenericFile(stream, fileName, length, parentArchive);

            var startPos = stream.Position;
            using BinaryReader reader = new(stream, Encoding.Default, true);
            uint signature = reader.ReadUInt32();
            stream.Position = startPos;

            return signature switch
            {
                AFSArchive.SIGNATURE => new AFSArchive(stream, fileName, length, parentArchive),
                HFU2Archive.SIGNATURE => new HFU2Archive(stream, fileName, length, parentArchive),
                TTHImageArchive.SIGNATURE => new TTHImageArchive(stream, fileName, length, parentArchive),
                PNGFile.SIGNATURE => new PNGFile(stream, fileName, length, parentArchive),
                _ => new GenericFile(stream, fileName, length, parentArchive)
            };
        }
    }
}
