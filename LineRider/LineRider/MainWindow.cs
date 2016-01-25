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
        private Engine engine;

        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            if (engine != null)
            {
                engine.Stop();
            }
        }

        private void pnlEngine_Paint(object sender, PaintEventArgs e)
        {
            if(engine != null)
            {
                engine.Stop();
            }

            engine = new Engine();
            engine.Start(pnlEngine.CreateGraphics());

            engine.SaveGame += SaveLineRiderGame;
            engine.LoadGame += LoadLineRiderGame;

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
                    engine.Rider.Position.X = Convert.ToInt32(Split[0]);
                    engine.Rider.Position.Y = Convert.ToInt32(Split[1]);
                    // Vorhandene Linien löschen
                    engine.Lines.Clear();

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
                        engine.Lines.Add(line);
                    }
                    Reader.Close();

                }
                engine.State = EngineStates.Editor;
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
                    Writer.WriteLine(engine.Rider.Position.X.ToString() + ";" + engine.Rider.Position.Y.ToString());
                    foreach (Line line in engine.Lines)
                    {
                    Writer.WriteLine(line.Start.X.ToString() + ";" + line.Start.Y.ToString() + "|" + line.End.X.ToString() + ";" + line.End.Y.ToString());
                    }
                    Writer.Flush();
                    Writer.Close();

                }
                engine.State = EngineStates.Editor;
            });
        }

        /// <summary>
        /// Bei Überfahrer des Panels mit der Maus wird eine Nachricht erzeugt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlEngine_MouseMove(object sender, MouseEventArgs e)
        {
            if (engine != null)
            {
                UI_Message Message = new UI_Message();
                Message.Position = e.Location;
                Message.Type = UI_Message.Clicktype.Move;
                engine.PlaceMessage(Message);
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (engine != null)
            {
                engine.Stop();
            }
        }

        private void pnlEngine_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (engine != null)
                {
                    UI_Message Message = new UI_Message();
                    Message.Position = e.Location;
                    Message.Type = UI_Message.Clicktype.Left;
                    engine.PlaceMessage(Message);
                }
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (engine != null)
                {
                    UI_Message Message = new UI_Message();
                    Message.Position = e.Location;
                    Message.Type = UI_Message.Clicktype.Right;
                    engine.PlaceMessage(Message);
                }
            }

        }

        private void pnlEngine_MouseUp(object sender, MouseEventArgs e)
        {
            if (engine != null)
            {
                UI_Message Message = new UI_Message();
                Message.Position = e.Location;
                Message.Type = UI_Message.Clicktype.Released;
                engine.PlaceMessage(Message);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (engine != null)
            {
                engine.MoveOrigin(e.KeyCode, 100);
            }
        }
    }
}
