using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineRider
{
    public partial class MainWindow : Form
    {
        private Engine Engine;

        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            if (Engine != null)
            {
                Engine.Stop();
            }
        }

        private void pnlEngine_Paint(object sender, PaintEventArgs e)
        {
            if(Engine != null)
            {
                Engine.Stop();
            }

            Engine = new Engine();
            Engine.Start(pnlEngine.CreateGraphics());
        }

        /// <summary>
        /// Bei Überfahrer des Panels mit der Maus wird eine Nachricht erzeugt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlEngine_MouseMove(object sender, MouseEventArgs e)
        {   
            UI_Message Message = new UI_Message();
            Message.Position = e.Location;
            Message.Type = UI_Message.Clicktype.Move;
            Engine.PlaceMessage(Message);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Engine != null)
            {
                Engine.Stop();
            }
        }

        private void pnlEngine_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                UI_Message Message = new UI_Message();
                Message.Position = e.Location;
                Message.Type = UI_Message.Clicktype.Left;
                Engine.PlaceMessage(Message);
            }

        }

        private void pnlEngine_MouseUp(object sender, MouseEventArgs e)
        {
            UI_Message Message = new UI_Message();
            Message.Position = e.Location;
            Message.Type = UI_Message.Clicktype.Released;
            Engine.PlaceMessage(Message);
        }
    }
}
