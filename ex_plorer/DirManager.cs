using ex_plorer.NTFS;
using ex_plorer.NTFS.Files;
using ex_plorer.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ex_plorer
{
    internal class DirManager
    {
        private MasterFileTable MFT;

        private const string DirIcon = "$dir";
        private const string FileIcon = "$file";

        internal static DriveInfo[] Drives { get; } = DriveInfo.GetDrives();

        private string Path;
        public NTFS.Files.Directory CurrentDir;

        internal static Dictionary<string, IconPair> IconDictionary { get; } = new Dictionary<string, IconPair>();

        internal ImageList LargeIcons { get; }
        internal ImageList SmallIcons { get; }

        internal List<string> IconsSet { get; }

        internal DirManager(MasterFileTable MFT, string path)
        {
            this.MFT = MFT;
            Path = path;
            CurrentDir = (NTFS.Files.Directory) MFT.GetFile(path);

            // icons
            IconsSet = new List<string>();
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
            IEnumerable<ListViewItem> items = CurrentDir.GetChilds().Select(iFile =>
            {
                ListViewItem item = null;
                bool isDirectory = false;

                if (iFile is NTFS.Files.File file)
                {
                    item = GetFileItem(file);
                }
                else if (iFile is NTFS.Files.Directory dir)
                {
                    item = GetDirItem(dir);
                    isDirectory = true;
                }

                return new { isDirectory, item };
            }).OrderByDescending(arg => arg.isDirectory).Select(arg => arg.item);

            return items;
        }

        internal ListViewItem GetFileItem(NTFS.Files.File file)
        {
            ListViewItem item = new ListViewItem(file.GetFileName());
            item.SubItems.AddRange(new[]
            {
                (file.GetFileSize() / 1024).ToString(),
                $"{file.GetFileExtension()} File",
                file.GetLastModify()
            });
            item.ImageKey = GetIconKey(file);
            item.Tag = file;

            return item;
        }

        internal ListViewItem GetDirItem(NTFS.Files.Directory dir)
        {
            ListViewItem item = new ListViewItem(dir.GetFileName());
            item.SubItems.AddRange(new[] {"", "Directory", dir.GetLastModify()});
            item.ImageKey = DirIcon;
            item.Tag = dir;

            return item;
        }

        internal string GetIconKey(NTFS.Files.File file)
        {
            string key;
            string ext = file.GetFileExtension();
            
            key = ext;
            if (!IconsSet.Contains(key))
            {
                ExtractIcon(key, file.GetFilePath());
            }

            return key;
        }

        private void ExtractIcon(string key, string path)
        {
            IconsSet.Add(key);

            Icon smallIcon, largeIcon;

            if (IconDictionary.TryGetValue(key, out IconPair icons))
            {
                smallIcon = icons.Small;
                largeIcon = icons.Large;
            }
            else
            {
                //TODO: type names
                string typeName = NativeIcon.GetIconsAndTypeName(path, out smallIcon, out largeIcon);
                IconDictionary.Add(key, new IconPair(smallIcon, largeIcon));
            }

            LargeIcons.Images.Add(key, largeIcon);
            SmallIcons.Images.Add(key, smallIcon);
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