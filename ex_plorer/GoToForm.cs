using ex_plorer.NTFS;
using System.IO;
using System.Windows.Forms;

namespace ex_plorer
{
    public partial class GotoForm : Form
    {
        public string Result { get; private set; }
        private MasterFileTable MFT;

        public GotoForm(MasterFileTable MFT)
        {
            InitializeComponent();
            this.MFT = MFT;
        }

        private void Go_Click(object sender, System.EventArgs e)
        {
            string path = PathBox.Text;
            if (MFT.Exists(path))
                Result = path;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
