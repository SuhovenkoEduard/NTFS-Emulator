using ex_plorer.NTFS.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ex_plorer
{
    public partial class EditForm : Form
    {
        protected bool saved;
        protected string fileName;
        protected string data;
        public string FileName
        {
            get => fileName;
            set 
            {
                fileName = value;
                FileNameLabel.Text = value;
            }
        }
        public string Data
        {
            get => data;
            set
            {
                data = value;
                textBox1.Text = value;
                textBox1.Select(value.Length, 0);
            }
        }
        public bool Saved
        {
            get => saved;
            set
            {
                saved = value;
                SavedLabel.Text = value ? "" : "*";
            }
        }
        public File File { get; private set; }
        
        public EditForm(File file, bool saved = true)
        {
            InitializeComponent();
            this.FileName = file.GetFileName();
            this.Data = file.Read();
            this.Saved = saved;
            this.File = file;
            SetTabWidth(this.textBox1, 1);
        }

        public void SaveFile()
        {
            if (File.TryWrite(Data))
            {
                File.Write(Data);
                Saved = true;
            } else
            {
                MessageBox.Show("Не удалось сохранить файл.");
                saved = false;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e) 
        {
            Data = textBox1.Text;
            Saved = false;
        }
        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
                SaveFile();
            if (e.KeyCode == Keys.W && e.Control)
                Close();
        }
        private void EditForm_KeyDown(object sender, KeyEventArgs e) => KeyDownHandler(sender, e);
        private void textBox1_KeyDown(object sender, KeyEventArgs e) => KeyDownHandler(sender, e);
        private void EditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (saved == false)
            {
                DialogResult result = MessageBox.Show("Сохранить файл?", "Save file", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                    SaveFile();
            }

            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }


        // tab width
        private const int EM_SETTABSTOPS = 0x00CB;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);
        public static void SetTabWidth(TextBox textbox, int tabWidth)
        {
            Graphics graphics = textbox.CreateGraphics();
            var characterWidth = (int)graphics.MeasureString("M", textbox.Font).Width;
            SendMessage(textbox.Handle, EM_SETTABSTOPS, 1, new int[] { tabWidth * characterWidth / 2 });
        }

    }
}
