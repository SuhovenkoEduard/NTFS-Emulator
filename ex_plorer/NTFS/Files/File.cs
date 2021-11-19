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
        public string FilePath { get; set; }
        public string LastModify { get; set; }

        public BlockStream stream;
        
        public File(MasterFileTable MFT, string fileName, string fileExtension = "", IFile parent = null)
        {
            this.MFT = MFT;
            this.Parent = parent;
            this.FileName = fileName;
            this.FileExtension = fileExtension;
            this.FilePath = GetFilePath();
            this.LastModify = DateTime.UtcNow.ToString();
            this.stream = new BlockStream();
        }

        // data
        public bool TryWrite(string data) => ((data.Length + Block1KB.SIZE - 1) / Block1KB.SIZE < MFT.freeMemory.Count * 1024);
        public void Write(string data)
        {
            MFT.DeallocMemory(stream.blocks);
            stream.blocks = MFT.AllocMemory((data.Length + Block1KB.SIZE - 1) / Block1KB.SIZE);
            stream.WriteData(data);
            LastModify = DateTime.UtcNow.ToString();
        }
        public string Read() => stream.ReadData();

        // by interface
        public void SetFilePath(string newFilePath)
        {
            var fileNameAndExtension = newFilePath.Substring(newFilePath.LastIndexOf('\\') + 1);
            var splitted = fileNameAndExtension.Split('.');
            var hasExt = fileNameAndExtension.Any(x => x == '.');
            FileName = (hasExt? splitted.First() : fileNameAndExtension);
            FileExtension = (hasExt? splitted.Last() : "");
            FilePath = newFilePath;
            Parent?.RemoveChild(this);
            Parent = MFT.GetParentDir(FilePath);
            Parent?.AddChild(this);
            LastModify = DateTime.UtcNow.ToString();
        }
        public BlockStream GetStream() => stream;
        public IFile GetParent() => Parent;
        public void SetParent(IFile parent) => Parent = parent;
        public string GetFileName() => FileName;
        public string GetFileNameExtension() => FileName + (string.IsNullOrEmpty(FileExtension) ? "" : "." + FileExtension);
        public string GetFilePath() =>
            $"{Parent?.GetFilePath()}{FileName}{(string.IsNullOrEmpty(FileExtension)? "" : "." + FileExtension)}";
        public string GetFileExtension() => FileExtension;
        public int GetFileSize() => stream.Size;
        public string GetLastModify() => LastModify;
        public void SetStream(BlockStream stream) => this.stream = stream;
        public IFile Clone(IFile parent = null)
        {
            string undefinedFileName = MFT.GetUndefinedFileNameByDir(FileName, FileExtension, false, parent);
            IFile result = new File(MFT, undefinedFileName, "", parent);
            MFT.files.Add(result);
            BlockStream clonedStream = stream.Clone(MFT);
            result.SetStream(clonedStream);
            return result;
        }
        public void SetLastModify(string lastModify) => this.LastModify = lastModify;
        
        // childs
        public IEnumerable<IFile> GetChilds() => null;
        public void SetChilds(IEnumerable<IFile> childs) { }
        public void AddChild(IFile child) { }
        public void RemoveChild(IFile child) { }
    }
}
