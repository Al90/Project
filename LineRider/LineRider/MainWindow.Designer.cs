namespace LineRider
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.pnlEngine = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlEngine
            // 
            this.pnlEngine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEngine.Location = new System.Drawing.Point(0, 0);
            this.pnlEngine.Name = "pnlEngine";
            this.pnlEngine.Size = new System.Drawing.Size(800, 600);
            this.pnlEngine.TabIndex = 0;
            this.pnlEngine.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlEngine_Paint);
            this.pnlEngine.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlEngine_MouseDown);
            this.pnlEngine.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlEngine_MouseMove);
            this.pnlEngine.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlEngine_MouseUp);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.pnlEngine);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.Text = "LineRider by S.Müller";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlEngine;
    }
}

