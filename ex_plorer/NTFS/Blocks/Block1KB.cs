using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS
{
    [Serializable]
    public class Block1KB : MemoryBlock
    {
        public static int SIZE = 1024;
        public Block1KB() : this("") { }
        public Block1KB(string data = "") 
            : base(SIZE, data) { }
        
        public void WriteSync(Block1KB block) => data = block.data;
    }
}
