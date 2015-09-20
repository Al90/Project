namespace Physics_Test
{
    partial class GameWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlPlayground = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlPlayground
            // 
            this.pnlPlayground.Location = new System.Drawing.Point(0, 0);
            this.pnlPlayground.Name = "pnlPlayground";
            this.pnlPlayground.Size = new System.Drawing.Size(800, 600);
            this.pnlPlayground.TabIndex = 0;
            this.pnlPlayground.Click += new System.EventHandler(this.pnlPlayground_Click);
            this.pnlPlayground.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPlayground_Paint);
            this.pnlPlayground.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlPlayground_MouseDown);
            this.pnlPlayground.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlPlayground_MouseMove);
            this.pnlPlayground.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlPlayground_MouseUp);
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 600);
            this.Controls.Add(this.pnlPlayground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(817, 639);
            this.Name = "GameWindow";
            this.Text = "Game Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPlayground;
    }
}

