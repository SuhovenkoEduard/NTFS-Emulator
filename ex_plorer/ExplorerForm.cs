using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ex_plorer.Properties;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ex_plorer.NTFS;
using ex_plorer.NTFS.Files;

namespace ex_plorer
{
    public partial class ExplorerForm : Form
    {
        private bool PUT_FLAG = false;
        private MasterFileTable MFT;
        private DirManager Manager { get; set; }

        private StatusBar statusBar;
        private StatusBarPanel itemsCount;
        private MenuItem[] viewModeItems;

        private List<MenuItem> selectionDependentItems = new List<MenuItem>();
        private List<MenuItem> clipboardDependentItems = new List<MenuItem>();


        public ExplorerForm(MasterFileTable MFT, string path, bool showStatusBar = true, View viewMode = View.LargeIcon)
        {
            this.MFT = MFT;
            InitializeComponent();
            SetUpUI(showStatusBar, viewMode);
            Init(path);
        }
        private void Init(string path)
        {
            Icon = Resources.dir;
            Text = path;
            this.Manager = new DirManager(MFT, path);
            folderView.LargeImageList = Manager.LargeIcons;
            folderView.SmallImageList = Manager.SmallIcons;
            GetAllFiles();
        }
        // [+]
        private void GetAllFiles()
        {
            itemsCount.Text = "Please wait...";
            ListViewItem[] items = Manager.GetAllFiles().ToArray();
            folderView.Items.Clear();
            folderView.Items.AddRange(items);
            itemsCount.Text = $"{folderView.Items.Count} object(s)";
        }
        // [+]
        private MenuItem[] GetDrivesMenu()
        {
            MenuItem[] items = new MenuItem[1];
            MenuItem item = new MenuItem("C:\\", GoToDirFromMenu);
            item.Tag = "C:\\";
            items[0] = item;
            return items;
        }
        // [+]
        private void SetUpUI(bool showStatusBar, View viewMode)
        {
            itemsCount = new StatusBarPanel();
            statusBar = new StatusBar
            {
                Dock = DockStyle.Bottom,
                Panels = {itemsCount},
                ShowPanels = true,
                SizingGrip = true,
                Visible = showStatusBar
            };
            Controls.Add(statusBar);
            folderView.View = viewMode;

            MenuItem[] goItems = {
                new MenuItem("-"),
                new MenuItem("&Copy Path", (sender, e) => Clipboard.SetText(Text)),
                new MenuItem("&Go To...", GoToPrompt, Shortcut.CtrlG),
                new MenuItem("&Up One Level", UpOneLevel, Shortcut.CtrlE),
            };
            MenuItem goMenu = new MenuItem("&Go", GetDrivesMenu());
            goMenu.MenuItems.AddRange(goItems);

            MenuItem viewMenu = new MenuItem("&View", new[]
            {
                new MenuItem("&Status Bar", ToggleStatusBar) { Checked = showStatusBar },
                new MenuItem("-"),
                new MenuItem("&Refresh", RefreshWindow, Shortcut.CtrlR), 
                new MenuItem("-"),
            });
            viewModeItems = new[] {
                new MenuItem("Large icons", ToggleFolderViewMode(View.LargeIcon))
                    { RadioCheck = true, Checked = folderView.View == View.LargeIcon },
                new MenuItem("List", ToggleFolderViewMode(View.List))
                    { RadioCheck = true, Checked = folderView.View == View.List },
                new MenuItem("Details", ToggleFolderViewMode(View.Details))
                    { RadioCheck = true, Checked = folderView.View == View.Details },
            };
            viewMenu.MenuItems.AddRange(viewModeItems);

            MenuItem putItem = new MenuItem("&Put", TriggerPut, Shortcut.CtrlX);
            selectionDependentItems.Add(putItem);
            MenuItem copyItem = new MenuItem("&Copy", TriggerCopy, Shortcut.CtrlC);
            selectionDependentItems.Add(copyItem);
            MenuItem pasteItem = new MenuItem("&Paste", TriggerPaste, Shortcut.CtrlV);
            clipboardDependentItems.Add(pasteItem);

            MenuItem editMenu = new MenuItem("&Edit", new[]
            {
                copyItem,
                pasteItem,
                putItem,
                new MenuItem("-"),
                new MenuItem("Select &All", SelectAll, Shortcut.CtrlA)
            });
            editMenu.Popup += UpdateSelectionDependentMenu;
            editMenu.Popup += UpdateClipboardDependentMenu;

            MenuItem deleteItem = new MenuItem("&Delete", TriggerDelete, Shortcut.Del);
            selectionDependentItems.Add(deleteItem);
            MenuItem renameItem = new MenuItem("&Rename", TriggerRename, Shortcut.F2);
            selectionDependentItems.Add(renameItem);

            MenuItem fileMenu = new MenuItem("&File", new[]
            {
                new MenuItem("&New File", TriggerNewFile, Shortcut.CtrlF),
                new MenuItem("&New Directory", TriggerNewFolder, Shortcut.CtrlD),
                new MenuItem("-"),
                deleteItem,
                renameItem,
                new MenuItem("-"),
                new MenuItem("&Close", (sender, e) => Close(), Shortcut.CtrlW),
            });
            fileMenu.Popup += UpdateSelectionDependentMenu;

            //TODO: Main menu
            Menu = new MainMenu(new[]
            {
                fileMenu,
                editMenu,
                viewMenu,
                goMenu,
            });
        }
        // [+]
        private void ChangePath(string path) => Init(path);
        // [+]
        private EventHandler ToggleFolderViewMode(View view)
        {
            return (sender, e) =>
            {
                folderView.View = view;
                foreach (MenuItem item in viewModeItems)
                {
                    item.Checked = false;
                }

                ((MenuItem) sender).Checked = true;
            };
        }
        // [+]
        private void SelectAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in folderView.Items)
            {
                item.Selected = true;
            }
        }
        // 
        private void TriggerNewFile(object sender, EventArgs e)
        {
            folderView.SelectedItems.Clear();
            string baseName = "New file";
            File file = MFT.CreateFile(baseName, "", Manager.CurrentDir);
            ListViewItem newItem = Manager.GetFileItem(file);
            folderView.Items.Add(newItem);
            newItem.Selected = true;
            newItem.BeginEdit();
        }
        // [+]
        private void TriggerNewFolder(object sender, EventArgs e)
        {
            folderView.SelectedItems.Clear();
            string baseName = "New folder";
            Directory directory = MFT.CreateDirectory(baseName, Manager.CurrentDir);
            ListViewItem newItem = Manager.GetDirItem(directory);
            folderView.Items.Add(newItem);
            newItem.Selected = true;
            newItem.BeginEdit();
        }
        // [+]
        private void CopyInClipboard()
        {
            if (folderView.SelectedItems.Count == 0) return;

            StringCollection fileNames = new StringCollection();
            foreach (ListViewItem item in folderView.SelectedItems)
            {
                IFile info = (IFile)item.Tag;
                fileNames.Add(info.GetFilePath());
            }

            Clipboard.SetFileDropList(fileNames);
        }
        // [+]
        private void TriggerPut(object sender, EventArgs e)
        {
            CopyInClipboard();
            PUT_FLAG = true;
        }
        // [+]
        private void TriggerCopy(object sender, EventArgs e)
        {
            CopyInClipboard();
            PUT_FLAG = false;
        }
        // [+]
        private void TriggerPaste(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsFileDropList()) return;

            folderView.SelectedItems.Clear();

            StringCollection fileNames = Clipboard.GetFileDropList();
            foreach (string fromPath in fileNames)
            {
                if (fromPath == Manager.CurrentDir.GetFilePath())
                {
                    MessageBox.Show("Нельзя вставить папку саму в себя.",
                        "Error", MessageBoxButtons.OK);
                    return;
                }
            }

            foreach (string fromPath in fileNames)
            {
                IFile oldFile = MFT.GetFile(fromPath);
                string toPath = Manager.CurrentDir.GetFilePath() + oldFile.GetFileNameExtension() + (oldFile is Directory? "\\" : "");
                if (fromPath == toPath) continue;
                if (PUT_FLAG)
                    toPath = MFT.RenameFile(fromPath, toPath);
                else
                    toPath = MFT.CopyFile(fromPath, toPath).GetFilePath();
                IFile newFile = MFT.GetFile(toPath);
                ListViewItem newItem;
                if (oldFile is File)
                {
                    newItem = Manager.GetFileItem((File)newFile);
                } else
                {
                    newItem = Manager.GetDirItem((Directory)newFile);
                }
                if (fromPath != toPath)
                {
                    folderView.Items.Add(newItem);
                    newItem.Selected = true;
                }
            }
            PUT_FLAG = false;                
        }
        // [+]
        private void TriggerDelete(object sender, EventArgs e)
        {
            int count = folderView.SelectedItems.Count;
            if (count == 0) return;

            string message = count > 1 ? $"these {count} files" : folderView.SelectedItems[0].Text;
            DialogResult result = MessageBox.Show($"Are you sure you want to delete {message}?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            foreach (ListViewItem item in folderView.SelectedItems)
            {
                NTFS.Files.IFile info = (NTFS.Files.IFile) item.Tag;
                folderView.Items.Remove(item);
                MFT.RemoveFile(info.GetFilePath());
            }
        }
        // [+]
        private void TriggerRename(object sender, EventArgs e)
        {
            if (folderView.SelectedItems.Count == 0) return;

            ListViewItem item = folderView.SelectedItems[0];
            item.BeginEdit();
        }
        // [+]
        private void UpdateSelectionDependentMenu(object sender, EventArgs e)
        {
            bool enabled = folderView.SelectedItems.Count > 0;
            foreach (MenuItem item in selectionDependentItems)
            {
                item.Enabled = enabled;
            }
        }
        // [+]
        private void UpdateClipboardDependentMenu(object sender, EventArgs e)
        {
            bool enabled = Clipboard.ContainsFileDropList();
            foreach (MenuItem item in clipboardDependentItems)
            {
                item.Enabled = enabled;
            }
        }
        // [+]
        private void RefreshWindow(object sender, EventArgs e) => GetAllFiles();
        // [+]
        private void UpOneLevel(object sender, EventArgs e)
        {
            if (Manager.CurrentDir.Parent == null) return;
            ChangePath(Manager.CurrentDir.Parent.GetFilePath());
        }
        // [+]
        private void GoToDirFromMenu(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string path = (string)item.Tag;
            ChangePath(path);
        }
        // [+]
        private void GoToPrompt(object sender, EventArgs e)
        {
            GotoForm goTo = new GotoForm(MFT);
            goTo.ShowDialog();

            if (goTo.DialogResult != DialogResult.OK) return;
            if (string.IsNullOrEmpty(goTo.Result))
            {
                MessageBox.Show("Invalid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ChangePath(goTo.Result);
        }
        // [+]
        private void ToggleStatusBar(object sender, EventArgs e)
        {
            if (statusBar.Visible) statusBar.Hide();
            else statusBar.Show();
            ((MenuItem)sender).Checked = statusBar.Visible;
        }
        // [+]
        private void FolderView_ItemActivate(object sender, EventArgs e)
        {
            if (folderView.SelectedItems.Count == 0) return;

            ListViewItem item = folderView.SelectedItems[0];
            IFile info = (IFile)item.Tag;

            if (info is File file)
            {
                try
                {
                    EditForm editForm = new EditForm(file);
                    editForm.ShowDialog();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не удалось открыть файл: " + exception.Message);
                }
            }
            else if (info is NTFS.Files.Directory dir)
            {
                ChangePath(dir.GetFilePath());
            }
        }
        // [+]
        private void ExplorerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }
        // [+]
        private void FolderView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem item = folderView.Items[e.Item];
            string newName = e.Label;
            if (string.IsNullOrWhiteSpace(newName))
            {
                e.CancelEdit = true;
                return;
            }
            
            IFile info = (IFile)item.Tag;
            if (info is File file)
            {
                MFT.RenameFile(file.GetFilePath(), Manager.CurrentDir.GetFilePath() + newName);
                folderView.Items[e.Item] = Manager.GetFileItem((File)info);
            }
            else if (info is Directory dir)
            {
                MFT.RenameFile(dir.GetFilePath(), Manager.CurrentDir.GetFilePath() + newName + "\\");
                folderView.Items[e.Item] = Manager.GetDirItem((Directory)info);
            }

            item = folderView.Items[e.Item];
            if (e.Label != info.GetFileNameExtension())
            {
                e.CancelEdit = true;
                item.Text = info.GetFileNameExtension();
                FolderView_AfterLabelEdit(sender, new LabelEditEventArgs(item.Index));
            }
        }
        // [+]
        private void FolderView_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = folderView.SelectedItems.Count > 0;

            foreach (MenuItem item in selectionDependentItems)
            {
                item.Enabled = enabled;
            }
        }
    }
}
