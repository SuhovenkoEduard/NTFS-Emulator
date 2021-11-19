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
        public int Size { get => blocks.Count * Block1KB.SIZE; }
        public List<Block1KB> blocks;
        
        public BlockStream() 
        {
            blocks = new List<Block1KB>();
        }
        
        public BlockStream(List<Block1KB> blocks = null)
        {
            this.blocks = blocks == null? new List<Block1KB>() : blocks;
        }
        
        public BlockStream Clone(MasterFileTable MFT) 
        {
            BlockStream stream = new BlockStream(MFT.AllocMemory(blocks.Count));
            for (int i = 0; i < blocks.Count; ++i)
                stream.blocks[i].WriteSync(blocks[i]);
            return stream;
        } 


        public void WriteData(string data)
        {
            int counter = 0;
            for (int i = 0; i < data.Length; i += Block1KB.SIZE)
            {
                int len = Math.Min(data.Length - i, Block1KB.SIZE);
                blocks[counter].data = data.Substring(i, len);
                counter += 1;
            }
        }
        public string ReadData() => blocks.Aggregate("", (sum, block) => sum + block.data);
    }
}