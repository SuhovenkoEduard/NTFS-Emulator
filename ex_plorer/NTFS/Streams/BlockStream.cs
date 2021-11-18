using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS
{
    [Serializable]
    public class BlockStream
    {
        public List<Block1KB> blocks;
        public BlockStream() 
        {
            blocks = new List<Block1KB>();
        }
        public BlockStream(List<Block1KB> blocks = null)
        {
            this.blocks = blocks == null? blocks : new List<Block1KB>();
        }
        public BlockStream Clone(MasterFileTable MFT) 
        {
            BlockStream stream = new BlockStream(MFT.AllocMemory(blocks.Count));
            for (int i = 0; i < blocks.Count; ++i)
                stream.blocks[0].WriteSync(blocks[i]);
            return stream;
        } 

        public int Size { get => blocks.Count * Block1KB.SIZE; }
    }
}