using ex_plorer.NTFS;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ex_plorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.VisualStyleState = VisualStyleState.NoneEnabled;
            Application.SetCompatibleTextRenderingDefault(false);

            string driveName = "C:";
            string path = driveName + "\\";
            MasterFileTable MFT = new MasterFileTable(100, driveName);

            EditForm editForm = new EditForm("input.txt", "hkgfhjfhjflghkfgh \t");
            //editForm.Show();

            ExplorerForm form = new ExplorerForm(MFT, path);
            form.Show();

            Application.Run();
        }
    }
}
