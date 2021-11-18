using ex_plorer.NTFS.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ex_plorer.NTFS
{
    public class MasterFileTable
    {
        protected List<Block1KB> freeMemory;
        protected List<Block1KB> busyMemory;
        protected List<IFile> files;

        public int FreeBlocks => freeMemory.Count;
        public int BusyBlocks => busyMemory.Count;
        public int AllBlocks => FreeBlocks + BusyBlocks;

        public MasterFileTable(int sizeInBlocks = 100, string diskName = "C:")
        {
            freeMemory = new List<Block1KB>(sizeInBlocks);
            busyMemory = new List<Block1KB>();
            files = new List<IFile>
            {
                new Directory(this, diskName, null)
            };
        }
        

        public IFile CreateFile(string fileName, string fileExtension = "", IFile parent = null)
        {
            IFile newFile = new File(this, fileName, fileExtension, parent);
            parent?.SetChilds(parent?.GetChilds().Append(newFile));
            files.Add(newFile);
            return newFile;
        }

        public IFile CreateDirectory(string directoryName, IFile parent = null)
        {
            IFile newDirectory = new Directory(this, directoryName, parent);
            parent?.SetChilds(parent?.GetChilds().Append(newDirectory));
            files.Add(newDirectory);
            return newDirectory;
        }

        public IFile CopyFile(string fromPath, string toPath)
        {
            IFile currentFile = files.Find(file => file.GetFilePath() == fromPath);
            IFile toDir = GetParentDir(toPath);
            if (fromPath == toPath) return currentFile;
            return currentFile.Clone(toDir);            
        }

        public bool RenameFile(string oldFilePath, string newFilePath)
        {
            if (files.Any(file => file.GetFilePath() == oldFilePath))
            {
                files
                    .Where(file => file.GetFilePath() == oldFilePath)
                    .ToList()
                    .ForEach(file => file.SetFilePath(newFilePath));
                return true;
            }
            else
            {
                MessageBox.Show("Файл для удаления не найден.");
                return false;
            }
        }
        public bool RemoveFile(string filePath)
        {
            if (files.Any(file => file.GetFilePath() == filePath))
            {
                files.RemoveAll(file => file.GetFilePath() == filePath);
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
            if (sizeInBlocks > FreeBlocks)
            {
                MessageBox.Show("Недостаточно свободной памяти.");
                return new List<Block1KB>();
            }
            List<Block1KB> allocatedMemory = freeMemory.Take(sizeInBlocks).ToList();
            freeMemory.RemoveRange(0, sizeInBlocks);
            busyMemory.AddRange(allocatedMemory);
            return allocatedMemory;
        }
    }
}
