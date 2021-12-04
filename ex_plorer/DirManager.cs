using ex_plorer.NTFS;
using ex_plorer.NTFS.Files;
using ex_plorer.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ex_plorer
{
    internal class DirManager
    {
        private MasterFileTable MFT;

        private const string DirIcon = "$dir";
        private const string FileIcon = "$file";

        //internal static DriveInfo[] Drives { get; } = DriveInfo.GetDrives();
        
        public Directory CurrentDir;
        
        internal ImageList LargeIcons { get; }
        internal ImageList SmallIcons { get; }

        internal DirManager(MasterFileTable MFT, string path)
        {
            this.MFT = MFT;
            CurrentDir = (Directory) MFT.GetFile(path);

            // icons
            LargeIcons = new ImageList
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit,
                Images =
                {
                    { DirIcon, new Icon(Resources.dir, 32, 32) },
                    { FileIcon, new Icon(Resources.file, 32, 32) }
                }
            };
            SmallIcons = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit,

                Images =
                {
                    { DirIcon, new Icon(Resources.dir, 16, 16) },
                    { FileIcon, new Icon(Resources.file, 16, 16) }
                }
            };
        }

        //TODO: This is slow, use Win32 function calls instead of DirectoryInfo
        internal IEnumerable<ListViewItem> GetAllFiles()
        {
            IEnumerable<ListViewItem> items = CurrentDir.GetChilds()?.Select(iFile =>
            {
                ListViewItem item = null;
                bool isDirectory = false;

                if (iFile is File file)
                {
                    item = GetFileItem(file);
                }
                else if (iFile is Directory dir)
                {
                    item = GetDirItem(dir);
                    isDirectory = true;
                }

                return new { isDirectory, item };
            }).OrderByDescending(arg => arg.isDirectory).Select(arg => arg.item);

            return items;
        }

        internal ListViewItem GetFileItem(File file)
        {
            ListViewItem item = new ListViewItem(file.GetFileNameExtension());
            item.SubItems.AddRange(new[]
            {
                (file.GetFileSize() / 1024).ToString() + " KB",
                $".{file.FileExtension} File",
                file.GetLastModify()
            });
            item.ImageKey = FileIcon;
            item.Tag = file;

            return item;
        }

        internal ListViewItem GetDirItem(Directory dir)
        {
            ListViewItem item = new ListViewItem(dir.GetFileName());
            item.SubItems.AddRange(new[] {"", "Directory", dir.GetLastModify()});
            item.ImageKey = DirIcon;
            item.Tag = dir;

            return item;
        }
    }

    internal struct IconPair
    {
        internal Icon Small;
        internal Icon Large;

        internal IconPair(Icon small, Icon large)
        {
            Small = small;
            Large = large;
        }
    }
}