using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS.Files
{
    public class SerializationFile
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string LastModify { get; set; }
        public BlockStream Stream { get; set; }
        public bool IsDirectory { get; set; }

        public SerializationFile() { }
    }
}
