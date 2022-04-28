using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    internal class FileFormatException : Exception
    {
        public FileFormatException() : base() { }
        public FileFormatException(string msg) : base(msg) { }
    }

    internal class SignatureException : FileFormatException
    {
        public SignatureException(string fileType) : base("Invalid signature for file format " + fileType) { }
        public SignatureException(string fileType, uint shouldBe, uint butIs) :
            base($"Invalid signature for file format {fileType}: excpected {shouldBe:#X08} but got {butIs:#X08}")
        { }
    }
}
