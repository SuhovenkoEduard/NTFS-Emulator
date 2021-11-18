using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex_plorer.NTFS.Files
{
    public class File : IFile
    {
        public MasterFileTable MFT { get; }
        public IFile Parent { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; private set; }
        public string LastModify { get; private set; }

        protected Dictionary<string, BlockStream> streams;
        public File(MasterFileTable MFT, string fileName, string fileExtension = "", IFile parent = null)
        {
            this.MFT = MFT;
            this.Parent = parent;
            this.FileName = fileName;
            this.FileExtension = fileExtension;
            this.FilePath = GetFilePath();
            this.LastModify = DateTime.UtcNow.ToString();
            this.streams = new Dictionary<string, BlockStream>();
        }

        // by interface
        public void SetFilePath(string newFilePath)
        {
            var fileNameAndExtension = newFilePath.Substring(newFilePath.LastIndexOf('\\') + 1);
            var splitted = fileNameAndExtension.Split('.');
            var hasExt = fileNameAndExtension.Any(x => x == '.');
            FileName = (hasExt? splitted.First() : fileNameAndExtension);
            FileExtension = (hasExt? splitted.Last() : "");
            FilePath = newFilePath;
            Parent = MFT.GetParentDir(FilePath);
        }
        public Dictionary<string, BlockStream> GetStreams() => streams;
        public IFile GetParent() => Parent;
        public string GetFileName() => FileName;
        public string GetFilePath() =>
            $"{Parent?.GetFilePath()}{FileName}{(string.IsNullOrEmpty(FileExtension)? "" : "." + FileExtension)}";
        public string GetFileExtension() => FileExtension;
        public int GetFileSize()
        {
            if (streams.TryGetValue("DATA", out BlockStream blocks))
                return blocks.Size;
            return 0;
        }
        public string GetLastModify() => LastModify;
        public IEnumerable<IFile> GetChilds() => new List<IFile>();
        public void SetStreams(Dictionary<string, BlockStream> streams) => this.streams = streams;
        public IFile Clone(IFile parent = null)
        {
            IFile result = new File(MFT, FileName, FileExtension, parent);
            Dictionary<string, BlockStream> clonedStreams = new Dictionary<string, BlockStream>();

            foreach (var pair in streams)
                clonedStreams[pair.Key] = pair.Value.Clone(MFT);

            result.SetStreams(clonedStreams);
            return result;
        }
        public void SetLastModify(string lastModify) => this.LastModify = lastModify;
        public void SetChilds(IEnumerable<IFile> childs) { }
    }
}
