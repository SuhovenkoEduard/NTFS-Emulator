using ex_plorer.NTFS.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ex_plorer.NTFS
{
    [Serializable]
    public class MasterFileTable
    {
        public List<Block1KB> freeMemory;
        public List<Block1KB> busyMemory;
        [XmlIgnore]
        public List<IFile> files;
        
        public MasterFileTable() { }
        public MasterFileTable(int sizeInBlocks = 100, string diskName = "C:")
        {
            freeMemory = new List<Block1KB>(sizeInBlocks);
            for (int i = 0; i < sizeInBlocks; ++i)
                freeMemory.Add(new Block1KB());

            busyMemory = new List<Block1KB>();
            files = new List<IFile>
            {
                new Directory(this, diskName, null)
            };
        }
        
        public string GetUndefinedFileNameByDir(string fileName, string fileExtension, bool isDirectory, IFile parent = null)
        {
            string undefinedFileName = fileName;
            string defaultPath = (parent == null ? "" : parent.GetFilePath());
            string extWithDot = (string.IsNullOrEmpty(fileExtension) ? "" : "." + fileExtension);
            string dirPath = defaultPath + fileName + (isDirectory? "\\" : extWithDot);
            int counter = 0;
            while (Exists(dirPath))
            {
                counter++;
                dirPath = defaultPath + fileName + $" ({counter})" + (isDirectory? "\\" : extWithDot);
                undefinedFileName = fileName + $" ({counter})";
            }
            return undefinedFileName;
        }
        public File CreateFile(string fileName, string fileExtension = "", IFile parent = null)
        {
            string undefinedFileName = GetUndefinedFileNameByDir(fileName, fileExtension, false, parent);
            File newFile = new File(this, undefinedFileName, fileExtension, parent);
            parent?.AddChild(newFile);
            files.Add(newFile);
            return newFile;
        }
        public Directory CreateDirectory(string directoryName, IFile parent = null)
        {
            string undefinedFileName = GetUndefinedFileNameByDir(directoryName, "", true, parent);
            Directory newDirectory = new Directory(this, undefinedFileName, parent);
            parent?.AddChild(newDirectory);
            files.Add(newDirectory);
            return newDirectory;
        }

        public IFile CopyFile(string fromPath, string toPath)
        {
            IFile currentFile = GetFile(fromPath);
            IFile toDir = GetParentDir(toPath);
            if (fromPath == toPath) return currentFile;
            IFile newFile = currentFile.Clone(toDir);
            toDir?.AddChild(newFile);
            return newFile;
        }
        public string RenameFile(string oldFilePath, string newFilePath)
        {
            string correctNewPosition = newFilePath;
            if (files.Any(file => file.GetFilePath() == newFilePath))
            {
                IFile file = GetFile(newFilePath);
                string fileName = file.GetFileName();
                string fileExtension = file.GetFileExtension();
                string extWithDot = (string.IsNullOrEmpty(fileExtension) ? "" : "." + fileExtension);
                IFile parent = file.GetParent();
                bool isDirectory = (file is Directory);
                string undefinedFileName = GetUndefinedFileNameByDir(fileName, fileExtension, isDirectory, parent);
                correctNewPosition = (parent == null ? "" : parent.GetFilePath()) + undefinedFileName 
                    + (isDirectory ? "\\" : extWithDot);
            }
            
            if (files.Any(file => file.GetFilePath() == oldFilePath))
            {
                IFile file = files.Find(iFile => iFile.GetFilePath() == oldFilePath);
                file.SetFilePath(correctNewPosition);
                return file.GetFilePath();
            }
            else
            {
                MessageBox.Show("Файл для перемеименования не найден.");
                return "";
            }
        }
        public bool RemoveFile(string filePath)
        {
            if (files.Any(file => file.GetFilePath() == filePath))
            {
                IFile toRemove = files.Find(file => file.GetFilePath() == filePath);
                toRemove.GetChilds()?.ToList().ForEach(file => this.RemoveFile(file.GetFilePath()));
                toRemove.GetParent()?.RemoveChild(toRemove);
                files.Remove(toRemove);
                return true;
            } else
            {
                MessageBox.Show("Файл для удаления не найден.");
                return false;
            }
        }
        public Directory GetParentDir(string filePath)
        {
            string dirPath;
            if (filePath[filePath.Length - 1] == '\\')
            {
                var words = filePath.Split('\\');
                dirPath = words.Take(words.Length - 2).Aggregate("", (sum, x) => sum + x + "\\");
            } else
            {
                dirPath = filePath.Substring(0, filePath.LastIndexOf('\\')) + "\\";
            }
            //List<string> filePaths = files.Select(x => x.GetFilePath()).ToList();
            return (Directory) files.Find(file => file.GetFilePath() == dirPath);
        }

        public IFile GetFile(string path) => files.Find(file => file.GetFilePath() == path);

        // memory
        public List<Block1KB> AllocMemory(int sizeInBlocks)
        {
            if (sizeInBlocks > freeMemory.Count)
            {
                MessageBox.Show("Недостаточно свободной памяти.");
                return null;
            }
            List<Block1KB> allocatedMemory = freeMemory.Take(sizeInBlocks).ToList();
            freeMemory.RemoveRange(0, sizeInBlocks);
            busyMemory.AddRange(allocatedMemory);
            return allocatedMemory;
        }
        public void DeallocMemory(List<Block1KB> blocks)
        {
            blocks.ForEach(block => busyMemory.Remove(block));
            blocks.ForEach(block => freeMemory.Add(block));
        }

        public bool IsPathRooted(string path) => (files[0].GetFilePath() == path);
        public bool Exists(string path) => (GetFile(path) != null);
    }
}
