using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS.Files
{
    public interface IFile
    {
        // file name
        string GetFileName();
        
        // file path
        string GetFilePath();
        void SetFilePath(string newFilePath);
        
        // file extension
        string GetFileExtension();
        
        // file size
        int GetFileSize();
        
        // last modify
        string GetLastModify();
        void SetLastModify(string lastModify);
        
        IFile Clone(IFile parent = null);
        
        // parent
        IFile GetParent();
        void SetParent(IFile parent);

        // childs
        IEnumerable<IFile> GetChilds();
        void SetChilds(IEnumerable<IFile> childs);
        void AddChild(IFile child);
        void RemoveChild(IFile child);
        
        // stream
        BlockStream GetStream();
        void SetStream(BlockStream stream);
    }
}
