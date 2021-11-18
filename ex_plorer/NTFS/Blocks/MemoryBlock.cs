using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS
{
    [Serializable]
    public class MemoryBlock
    {
        public int SizeInB { get; }
        public int SizeInKB { get => SizeInB / 1024; }
        public string data;

        public MemoryBlock() { }

        public MemoryBlock(int sizeInB, string data = "")
        {
            this.SizeInB = sizeInB;
            this.data = data;

            while (this.data.Length < sizeInB)
                this.data += '0';
            if (this.data.Length > sizeInB)
                this.data = this.data.Substring(0, sizeInB);
        }
    }
}
