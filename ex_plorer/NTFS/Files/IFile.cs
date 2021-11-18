using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS.Files
{
    public interface IFile
    {
        string GetFileName();
        string GetFilePath();
        void SetFilePath(string newFilePath);
        string GetFileExtension();
        int GetFileSize();
        string GetLastModify();
        void SetLastModify(string lastModify);
        IFile Clone(IFile parent = null);
        IFile GetParent();
        IEnumerable<IFile> GetChilds();
        void SetChilds(IEnumerable<IFile> childs);
        BlockStream GetStream();
        void SetStream(BlockStream stream);
    }
}
