using ex_plorer.NTFS;
using ex_plorer.NTFS.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;

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

            string restorePath = "../../../Files.xml";
            string driveName = "C:";
            string path = driveName + "\\";
            MasterFileTable MFT = new MasterFileTable(100, driveName);
            Serializer serializer = new Serializer(MFT, restorePath);
            if (System.IO.File.Exists(restorePath))
                MFT.files = serializer.Deserialize();

            ExplorerForm form = new ExplorerForm(MFT, path);
            form.Show();

            Application.Run();
        }
    }
}
