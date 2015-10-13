using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Physics_Test
{
    public partial class GameWindow : Form
    {
        private Engine MyEngine;
        private int LastCreation;

        public GameWindow()
        {
            InitializeComponent();
        }

        private void pnlPlayground_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = pnlPlayground.CreateGraphics();
            MyEngine = new Engine(g);
            MyEngine.start();
        }

        private void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MyEngine != null)
            {
                MyEngine.stop();
            }
        }

        private void pnlPlayground_Click(object sender, EventArgs e)
        {
            //MyEngine.createBall(PointToClient(MousePosition));
        }

        private void pnlPlayground_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (Environment.TickCount > LastCreation + 20)
                {
                    // save last creation
                    LastCreation = Environment.TickCount;
                    
                    // create ball
                    if (MyEngine != null)
                    {
                        //MyEngine.createBall(PointToClient(MousePosition));
                    }
                }

                MyEngine.MoveArrow(PointToClient(MousePosition));
            }
            if (MyEngine != null)
            {
                MyEngine.MoveStartPos(PointToClient(MousePosition));
            }
        }

        private void pnlPlayground_MouseDown(object sender, MouseEventArgs e)
        {
            MyEngine.StartArrow(PointToClient(MousePosition));
        }

        private void pnlPlayground_MouseUp(object sender, MouseEventArgs e)
        {
            MyEngine.EndArrow();
        }

        private void pnlPlayground_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void pnlPlayground_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }
    }
}
