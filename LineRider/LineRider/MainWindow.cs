using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

            Engine.SaveGame += SaveLineRiderGame;
            Engine.LoadGame += LoadLineRiderGame;

        }

        private void LoadLineRiderGame(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Datei zum laden wählen
                OpenFileDialog Dialog;
                Dialog = new OpenFileDialog();
                Dialog.AddExtension = true;
                Dialog.CheckFileExists = true;
                Dialog.CheckPathExists = true;
                Dialog.DefaultExt = "simon";
                Dialog.Filter = "LineRider files (*.simon)|*.simon";
                Dialog.InitialDirectory = Directory.GetCurrentDirectory();
                Dialog.Title = "Strecke laden";

                // Dialog anzeigen
                if (Dialog.ShowDialog() == DialogResult.OK)
                {
                    // Datei laden
                    StreamReader Reader;
                    Reader = new StreamReader(Dialog.FileName);
                    string[] Split = Reader.ReadLine().Split(';');
                    Engine.Rider.Position.X = Convert.ToInt32(Split[0]);
                    Engine.Rider.Position.Y = Convert.ToInt32(Split[1]);
                    // Vorhandene Linien löschen
                    Engine.Lines.Clear();

                    while(Reader.EndOfStream == false)
                    {
                        string[] Element = Reader.ReadLine().Split('|');
                        string[] Start = Element[0].Split(';');
                        string[] End = Element[1].Split(';');
                        Line line = new Line();
                        line.Start.X = Convert.ToInt32(Start[0]);
                        line.Start.Y = Convert.ToInt32(Start[1]);
                        line.End.X = Convert.ToInt32(End[0]);
                        line.End.Y = Convert.ToInt32(End[1]);
                        line.Calculate();
                        Engine.Lines.Add(line);
                    }
                    Reader.Close();

                }
                Engine.State = EngineStates.Editor;
            });
        }

        private void SaveLineRiderGame(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Datei zum speichern wählen
                SaveFileDialog Dialog;
                Dialog = new SaveFileDialog();
                Dialog.AddExtension = true;
                Dialog.CheckFileExists = false;
                Dialog.CheckPathExists = false;
                Dialog.CreatePrompt = false;
                Dialog.DefaultExt = "simon";
                Dialog.Filter = "LineRider files (*.simon)|*.simon";
                Dialog.InitialDirectory = Directory.GetCurrentDirectory();
                Dialog.OverwritePrompt = true;
                Dialog.Title = "Strecke speichern";

                // Dialog anzeigen
                if (Dialog.ShowDialog() == DialogResult.OK)
                {
                    // Datei speichern
                    StreamWriter Writer;
                    Writer = new StreamWriter(Dialog.FileName);
                    Writer.WriteLine(Engine.Rider.Position.X.ToString() + ";" + Engine.Rider.Position.Y.ToString());
                    foreach (Line line in Engine.Lines)
                    {
                    Writer.WriteLine(line.Start.X.ToString() + ";" + line.Start.Y.ToString() + "|" + line.End.X.ToString() + ";" + line.End.Y.ToString());
                    }
                    Writer.Flush();
                    Writer.Close();

                }
                Engine.State = EngineStates.Editor;
            });
        }

        /// <summary>
        /// Bei Überfahrer des Panels mit der Maus wird eine Nachricht erzeugt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlEngine_MouseMove(object sender, MouseEventArgs e)
        {
            if (Engine != null)
            {
                UI_Message Message = new UI_Message();
                Message.Position = e.Location;
                Message.Type = UI_Message.Clicktype.Move;
                Engine.PlaceMessage(Message);
            }
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
                if (Engine != null)
                {
                    UI_Message Message = new UI_Message();
                    Message.Position = e.Location;
                    Message.Type = UI_Message.Clicktype.Left;
                    Engine.PlaceMessage(Message);
                }
            }

        }

        private void pnlEngine_MouseUp(object sender, MouseEventArgs e)
        {
            if (Engine != null)
            {
                UI_Message Message = new UI_Message();
                Message.Position = e.Location;
                Message.Type = UI_Message.Clicktype.Released;
                Engine.PlaceMessage(Message);
            }
        }
    }
}
