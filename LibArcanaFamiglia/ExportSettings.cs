using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    public class ExportSettings
    {
        public bool AddDefaultExtension = true;
        public bool ReduceFolders = true;
        public bool ConvertImages = false;
        public string RootPath = Environment.CurrentDirectory;

        public ExportSettings() { }
        public ExportSettings(string path) => RootPath = path;
    }
}
