namespace Analyse
{
    partial class Hauptfenster
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
            this.pnlEngine = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlEngine
            // 
            this.pnlEngine.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlEngine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEngine.Location = new System.Drawing.Point(0, 0);
            this.pnlEngine.Name = "pnlEngine";
            this.pnlEngine.Size = new System.Drawing.Size(784, 561);
            this.pnlEngine.TabIndex = 0;
            this.pnlEngine.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlEngine_Paint);
            this.pnlEngine.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlEngine_MouseDown);
            this.pnlEngine.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlEngine_MouseMove);
            this.pnlEngine.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlEngine_MouseUp);
            // 
            // Hauptfenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.pnlEngine);
            this.Name = "Hauptfenster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Hauptfenster_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlEngine;
    }
}

