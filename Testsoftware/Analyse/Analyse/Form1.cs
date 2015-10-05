using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analyse
{
    public partial class Hauptfenster : Form
    {
        /// <summary>
        /// Grafik Engine
        /// </summary>
        private Engine engine; 

        /// <summary>
        /// Konstruktor des Hauptfensters
        /// </summary>
        public Hauptfenster()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Paint Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlEngine_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = pnlEngine.CreateGraphics(); // Grafikhandle des Panels holen
            engine = new Engine(g); // Neue Engine erzeugen
            engine.Start(); // Engine starten

        }

        /// <summary>
        /// Schliessen Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hauptfenster_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(engine != null) // Prüfen ob Engine vorhanden ist
            {
                engine.Stop(); // Engine stoppen
            }
        }

        private void pnlEngine_MouseMove(object sender, MouseEventArgs e)
        {
            if (engine != null) // Prüfen ob Engine vorhanden ist
            {
                engine.Cursor = e.Location; // Mausposition an ENgine übergeben
            }
        }
    }
}
