using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS.Files
{
    public class Directory : IFile
    {
        public MasterFileTable MFT { get; }
        public IFile Parent { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; }
        public string FilePath { get; private set; }
        public string LastModify { get; private set; }

        protected Dictionary<string, BlockStream> streams;
        protected List<IFile> childs;

        public Directory(MasterFileTable MFT, string fileName, IFile parent = null)
        {
            this.MFT = MFT;
            this.Parent = parent;
            this.FileName = fileName;
            this.FileExtension = "";
            this.FilePath = GetFilePath();
            this.LastModify = DateTime.UtcNow.ToString();
            this.streams = new Dictionary<string, BlockStream>();
            this.childs = new List<IFile>();
        }


        // by interface
        public string GetFileName() => FileName;
        public string GetFilePath() =>
            $"{Parent?.GetFilePath()}{FileName}\\";

        public void SetFilePath(string newFilePath)
        {
            FileName = newFilePath.Split('\\').Reverse().Skip(1).First();
            FilePath = newFilePath;
            Parent = MFT.GetParentDir(FilePath);
        }
        public string GetFileExtension() => FileExtension;
        public int GetFileSize() => GetChilds().Aggregate(0, (sum, x) => sum + x.GetFileSize());
        public string GetLastModify() => LastModify;

        public IFile Clone(IFile parent = null)
        {
            IFile clonedDir = new Directory(MFT, FileName, parent);
            clonedDir.SetChilds(childs.Select(file => file.Clone(this)));
            return clonedDir;
        }

        public IFile GetParent() => this.Parent;
        public IEnumerable<IFile> GetChilds() => this.childs;
        public Dictionary<string, BlockStream> GetStreams() => this.streams;
        public void SetStreams(Dictionary<string, BlockStream> streams) => this.streams = streams;

        public void SetLastModify(string lastModify) => this.LastModify = lastModify;

        public void SetChilds(IEnumerable<IFile> childs) => this.childs = childs.ToList();        
    }
}
