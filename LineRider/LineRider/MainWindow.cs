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
    }
}
