using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace LineRider
{
    public class Engine
    {
        /// <summary>
        /// Alle Linien
        /// </summary>
        private List<Line> Lines;
        /// <summary>
        /// Spieler
        /// </summary>
        private Player Rider;
        /// <summary>
        /// Allgemeine Beschleunigung
        /// </summary>
        private double Acceleration;
        /// <summary>
        /// Thread in dem die einzelnen Frames des Spiels gerechnet werden
        /// </summary>
        private Thread Render;
        /// <summary>
        /// Handle zur Grafik welche mit den Frames überzeichnet wird
        /// </summary>
        private Graphics g;
        /// <summary>
        /// Spieldauer
        /// </summary>
        private DateTime Playtime;
        /// <summary>
        /// Alle Buttons
        /// </summary>
        private List<GameButton> GameButtons;
        /// <summary>
        /// Ursprung
        /// </summary>
        private Point Origin;
        /// <summary>
        /// Gerendertes Frame
        /// </summary>
        private Bitmap Frame;
        /// <summary>
        /// Nachrichtenqueue (Postfach) für Benutzereingaben
        /// </summary>
        private Queue<UI_Message> Messages;

        private GameButton Play;
        private GameButton Pause;
        private GameButton Save;
        private GameButton Load;
        private Point Offset;
        /// <summary>
        /// Globaler Spielzustand
        /// </summary>
        private EngineStates State;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Engine()
        {
            Lines = new List<Line>();
            Rider = new Player(new Point(100,500),60,global::LineRider.Properties.Resources.skate_board_307418_640);
            Acceleration = 1.0;
            Playtime = new DateTime();
            GameButtons = new List<GameButton>();
            Origin = new Point();
            Messages = new Queue<UI_Message>();
            Offset = new Point(0, 600);
            int Size = 40;
            Play = new GameButton(new Point((int)(800 / 2 - (2 * Size + 1.5 * 10)), 10), Size, true, global::LineRider.Properties.Resources.Button_Play_icon);
            Pause = new GameButton(new Point((int)(800 / 2 - (1 * Size + 0.5 * 10)), 10), Size, true, global::LineRider.Properties.Resources.Button_Pause_icon);
            Save = new GameButton(new Point((int)(800 / 2 + (0.5 * 10)), 10), Size, true, global::LineRider.Properties.Resources.Save_icon);
            Load = new GameButton(new Point((int)(800 / 2 + (1 * Size + 1.5 * 10)), 10), Size, true, global::LineRider.Properties.Resources.Open_Folder_icon);
            GameButtons.Add(Play);
            GameButtons.Add(Pause);
            GameButtons.Add(Save);
            GameButtons.Add(Load);
            State = EngineStates.Editor;
        }



        /// <summary>
        /// Grafik berechnen
        /// </summary>
        /// <param name="obj">Optionales Threadobjekt</param>
        private void RenderGraphics(object obj)
        {
            Frame = new Bitmap(800, 600);
            Graphics f_handle = Graphics.FromImage(Frame);
            Pause.Clicked = true;
            bool flag_linestart = false;
            bool flag_lineend = false;
            Line Editorline = new Line();

            while(true)
            {
                #region Spielzustand

                // Spielzustand
                switch(State)
                {
                    default:
                    case EngineStates.Editor:
                        Load.Enabled = true;
                        Save.Enabled = true;
                        Pause.Clicked = true;

                        if(flag_lineend)
                        {
                            Lines.Add(Editorline);
                            Editorline = new Line();
                            flag_lineend = false;
                            flag_linestart = false;

                        }
                        break;
                    
                    case EngineStates.Load:
                        Load.Enabled = true;
                        Save.Enabled = true;
                        Load.Clicked = true;
                        break;

                    case EngineStates.Run:
                        Load.Enabled = false;
                        Save.Enabled = false;
                        Play.Clicked = true;
                        break;

                    case EngineStates.Save:
                        Load.Enabled = true;
                        Save.Enabled = true;
                        Save.Clicked = true;
                        break;
                }

                #endregion

                #region Zeichnen

                // Berechnungen anstellen
                // Hintergrund zeichnen
                f_handle.Clear(Color.Orange);

                // Linien zeichnen
                Lines.ForEach(x=>x.Draw(f_handle, Offset, Origin));

                // Editorlinie zeichnen
                if(flag_linestart)
                {
                    Editorline.Draw(f_handle, Offset, Origin);
                }

                // Spieler zeichnen
                Rider.Draw(f_handle, Offset, Origin);

                // Spielzeitzähler 
                // Menu
                GameButtons.ForEach(x => x.Draw(f_handle));

                // Frame zeichnen
                g.DrawImage(Frame, 0, 0);

                #endregion

                #region Nachrichtenverarbeitung

                // Nachrichtenpostfach abarbeiten
                while(Messages.Count > 0) // Anzahl der Nachrichten wird gezählt
                {
                    // Nachricht aus Postfach holen
                    UI_Message Message = Messages.Dequeue();
                    // Schauen, welcher Button geklickt wurde
                    GameButtons.ForEach(x => x.Handle_UI(Message));
                    if(Play.Clicked)
                    {
                        State = EngineStates.Run;
                    }
                    else
                    {
                        if (Pause.Clicked)
                        {
                            State = EngineStates.Editor;
                        }
                        else
                        {
                            if (Load.Clicked)
                            {
                                State = EngineStates.Load;
                            }
                            else
                            {
                                if (Save.Clicked)
                                {
                                    State = EngineStates.Save;
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                    }

                    // Wenn im State Editor, vom Benutzer eingegebene Punkte zu einer Linie umrechnen
                    if(State == EngineStates.Editor)
                    {
                        switch (Message.Type)
                        {
                                // Mit einem Linksklick wird der Start einer Linie markiert
                            case UI_Message.Clicktype.Left:
                                flag_linestart = true;
                                Editorline.Start.X = Offset.X + Origin.X + Message.Position.X;
                                Editorline.Start.Y = Offset.Y - Origin.Y - Message.Position.Y;
                                Editorline.End.X = Editorline.Start.X;
                                Editorline.End.Y = Editorline.Start.Y;
                                break;

                                // Mit dem bewegen der Maus wird der Endpunkt verschoben 
                            case UI_Message.Clicktype.Move:
                                if (flag_linestart)
                                {
                                    Editorline.End.X = Offset.X + Origin.X + Message.Position.X;
                                    Editorline.End.Y = Offset.Y - Origin.Y - Message.Position.Y;
                                }
                                break;

                                // Mit Loslassen der Linkstaste wird der Enpunkt der Linie gesetzt
                            case UI_Message.Clicktype.Released:
                                if (flag_linestart)
                                {
                                    Editorline.End.X = Offset.X + Origin.X + Message.Position.X;
                                    Editorline.End.Y = Offset.Y - Origin.Y - Message.Position.Y;
                                    flag_lineend = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // Flags zurücksetzen
                        flag_linestart = false;
                        flag_lineend = false;
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Engine anhalten
        /// </summary>
        public void Stop()
        {
            if ((Render != null) && (Render.IsAlive == true))
            {
                Render.Abort();
            }
        }

        /// <summary>
        /// Nachricht in Queue legen
        /// </summary>
        /// <param name="Message">Abzulegende Nachricht</param>
        public void PlaceMessage(UI_Message Message)
        {
            Messages.Enqueue(Message); // Nachricht in Postfach legen
        }

        public void Start(Graphics graphics)
        {
            Stop();
            g = graphics;
            Render = new Thread(RenderGraphics);
            Render.Name = "EngineRender";
            Render.Start();


        }
    }
}
