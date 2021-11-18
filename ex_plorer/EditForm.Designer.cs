
namespace ex_plorer
{
    partial class EditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.SavedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.AcceptsTab = true;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(12, 48);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(776, 390);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FileNameLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FileNameLabel.Location = new System.Drawing.Point(12, 19);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(119, 21);
            this.FileNameLabel.TabIndex = 1;
            this.FileNameLabel.Text = "filename.ext\r\n";
            // 
            // SavedLabel
            // 
            this.SavedLabel.AutoSize = true;
            this.SavedLabel.BackColor = System.Drawing.Color.Transparent;
            this.SavedLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SavedLabel.Location = new System.Drawing.Point(137, 21);
            this.SavedLabel.Name = "SavedLabel";
            this.SavedLabel.Size = new System.Drawing.Size(18, 19);
            this.SavedLabel.TabIndex = 1;
            this.SavedLabel.Text = "*";
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SavedLabel);
            this.Controls.Add(this.FileNameLabel);
            this.Controls.Add(this.textBox1);
            this.Name = "EditForm";
            this.Text = "EditForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditForm_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.Label SavedLabel;
    }
}