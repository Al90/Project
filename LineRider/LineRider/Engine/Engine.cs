using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LineRider
{
    public class Engine
    {
        /// <summary>
        /// Alle Linien
        /// </summary>
        public List<Line> Lines;
        /// <summary>
        /// Spieler
        /// </summary>
        public Player Rider;
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
        private TimeSpan Playtime;
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
        public EngineStates State;

        public EventHandler SaveGame;
        public EventHandler LoadGame;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Engine()
        {
            Lines = new List<Line>();
            Rider = new Player(new Point(100,500),60,global::LineRider.Properties.Resources.skate_board_307418_640);
            Acceleration = 1.0;
            Playtime = new TimeSpan();
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
        [STAThread]
        private void RenderGraphics(object obj)
        {
            Frame = new Bitmap(800, 600);
            Graphics f_handle = Graphics.FromImage(Frame);
            Pause.State = true;
            bool flag_linestart = false;
            bool flag_lineend = false;
            Line Editorline = new Line();
            int TimeStamp = 0;
            int Ground = 0;
            DateTime Starttime = DateTime.Now;
            Font Time_f = new Font("Arial",24f);
            Brush Time_b = new SolidBrush(Color.Blue);
          

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
                        Pause.State = true;
                        Pause.Enabled = true;
                        Play.Enabled = true;
                        Load.State = false;
                        Save.State = false;
                        Pause.State = true;
                        Play.State = false;

                        if(flag_lineend)
                        {
                            Editorline.Calculate();
                            Lines.Add(Editorline);
                            Editorline = new Line();
                            flag_lineend = false;
                            flag_linestart = false;

                        }
                        break;
                    
                    case EngineStates.Load:
                        Load.Enabled = false;
                        Save.Enabled = false;
                        Load.State = true;
                        Pause.Enabled = false;
                        Play.Enabled = false;
                        Save.State = false;
                        Pause.State = false;
                        Play.State = false;
                        break;

                    case EngineStates.Run:
                        Load.Enabled = false;
                        Save.Enabled = false;
                        Play.State = true;
                        Pause.Enabled = true;
                        Play.Enabled = true;
                        Load.State = false;
                        Save.State = false;
                        Pause.State = false;
                        break;

                    case EngineStates.Save:
                        Load.Enabled = false;
                        Save.Enabled = false;
                        Save.State = true;
                        Pause.Enabled = false;
                        Play.Enabled = false;
                        Load.State = false;
                        Pause.State = false;
                        Play.State = false;
                        break;
                }

                #endregion

                #region Zeichnen

                // Berechnungen anstellen
                if ((System.Environment.TickCount >= (TimeStamp + 100)) && (State == EngineStates.Run))
                {
                    // TimeStamp aktualisieren
                    TimeStamp = System.Environment.TickCount; 

                    // Fahrdauer rechnen
                    Playtime = DateTime.Now - Starttime;

                    // Geschwindigkeit des Spielers rechnen
                    //Rider.Speed = 10;
                    if (Rider.Contacted == null)
                    {
                        // Freier Fall
                        double y_speed = Rider.Speed * Math.Sin(Rider.Angle / 360 * 2 * Math.PI) - 1;
                        double x_speed = Rider.Speed * Math.Cos(Rider.Angle / 360 * 2 * Math.PI);
                        Rider.Speed = Math.Sqrt((y_speed * y_speed) + (x_speed * x_speed));

                        if (Rider.Speed > 10)
                        {
                            double factor = 10 / Rider.Speed;
                            y_speed = y_speed * factor;
                            x_speed = x_speed * factor;
                            Rider.Speed = 10;
                        }

                        if (Rider.Speed == 0)
                        {
                            Rider.Angle = 270;
                        }
                        else
                        {
                            if (x_speed > 0)
                            {
                                Rider.Angle = (Math.Asin(y_speed / Rider.Speed) * 360 / (2 * Math.PI)) % 360;
                            }
                            else
                            {
                                Rider.Angle = (180 - Math.Asin(y_speed / Rider.Speed) * 360 / (2 * Math.PI)) % 360;
                            }
                        }
                    }
                    else
                    {
                        // Y-Komponente der Geschwindigkeit
                        double y_speed = Rider.Speed * Math.Sin(Rider.Angle / 360 * 2 * Math.PI) - Math.Sin(Rider.Angle / 360 * 2 * Math.PI);
                        // X-Komponente der Geschwindigkeit
                        double x_speed = Rider.Speed * Math.Cos(Rider.Angle / 360 * 2 * Math.PI);
                        double factor = 0;
                        if ((Rider.Angle % 90) != 0)
                        {
                            double angle = Rider.Angle % 45;
                            if (angle == 0)
                            {
                                factor = 0.5;
                            }
                            else
                            {
                                factor = Math.Sin(angle / 360 * 2 * Math.PI);
                            }
                            if (((Rider.Angle > 0) && (Rider.Angle < 90)) || ((Rider.Angle > 180) && (Rider.Angle < 270)))
                            {
                                x_speed -= factor;
                            }
                            else
                            {
                                x_speed += factor;
                            }
                        }
                        Rider.Speed = Math.Sqrt((y_speed * y_speed) + (x_speed * x_speed));
                        if (Rider.Speed < 0.5)
                        {
                            Rider.Angle = (Rider.Angle + 180) % 360;
                        }
                    }

                    // Kontaktierte Linie berechnen
                    double SmallestDistance = double.MaxValue;
                    double Distance = double.MaxValue;
                    foreach(Line L in Lines)
                    {
                        // Prüfen ob Linie in der Nähe des Spielers ist
                        if (inRange(Rider.Position, L))
                        {
                            // Distanz zu Linie rechnen
                            Distance = getDistanceF(Rider.Position, L);

                            // Prüfen ob Distanz kleiner als kleinste Distanz und kleiner als 5
                            if ((Distance < SmallestDistance) && (Distance < 5) && (Rider.Contacted != L))
                            {
                                // Kleinste Distanz aktualisieren
                                SmallestDistance = Distance;

                                Rider.Speed = Rider.Speed * 0.5;

                                // Spielerwinkel rechnen
                                if (L.Angle <= 90)
                                {
                                    if ((L.Angle - (Rider.Angle % 180) >= 90))
                                    {
                                        Rider.Angle = (L.Angle + 180) % 360;
                                    }
                                    else
                                    {
                                        Rider.Angle = L.Angle;
                                    }
                                }
                                else
                                {
                                    if ((L.Angle - (Rider.Angle % 180) >= 90))
                                    {
                                        Rider.Angle = L.Angle;
                                    }
                                    else
                                    {
                                        Rider.Angle = (L.Angle + 180) % 360;
                                    }
                                }

                                // Kontaktierte Linie setzen
                                Rider.Contacted = L;
                            }

                            if (Rider.Contacted == L)
                            {
                                if (Distance > 10)
                                {
                                    Rider.Contacted = null;
                                }
                            }
                        }
                        else
                        {
                            if (Rider.Contacted == L)
                            {
                                Rider.Contacted = null;
                            }
                        }
                    }

                    // Abfrage Linie kontaktiert
                    if (Rider.Contacted != null)
                    {
                        // Winkel der Linie kopieren
                        // Rider.Angle = Rider.Contacted.Angle;
                        Rider.Contacted.Color = Color.Red;
                    }
                    
                    // Alle nicht kontaktierten Linien schwarz färben
                    Lines.ForEach(x =>
                    {
                        if (x != Rider.Contacted)
                        {
                            x.Color = Color.Black;
                        }
                    });
                    

                    // Verschiebung in X rechnen
                    Rider.Position.X += (float)(Rider.Speed * Math.Cos(Rider.Angle * Math.PI / 180));

                    // Verschiebung in Y rechnen
                    Rider.Position.Y += (float)(Rider.Speed * Math.Sin(Rider.Angle * Math.PI / 180));

                    // Feststellen ob Spiel zu ende ist
                    if (Rider.Position.Y < Ground)
                    {
                        State = EngineStates.Editor;
                        Rider.Position.X = 100;
                        Rider.Position.Y = 500;
                        Rider.Angle = 270f;
                        Rider.Speed = 0;
                        Play.Clicked = false;
                    }
                }


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
                f_handle.DrawString(Playtime.ToString("hh\\:mm\\:ss"), Time_f, Time_b, 10f, 556f);

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
                        if (State != EngineStates.Run)
                        {
                            State = EngineStates.Run;
                            Starttime = DateTime.Now;
                            if (Lines.Count == 0)
                            {
                                Ground = 0;
                            }
                            else
                            {
                                Ground = Lines.Min(x => x.Start.Y);
                                if (Ground > Lines.Min(x => x.End.Y))
                                {
                                    Ground = Lines.Min(x => x.End.Y);
                                }
                                Ground -= 100;
                            }
                        }
                    }
                    else
                    {
                        if (Pause.Clicked)
                        {
                            State = EngineStates.Editor;
                            Rider.Position.X = 100;
                            Rider.Position.Y = 500;
                            Rider.Angle = 270f;
                            Rider.Speed = 0;
                        }
                        else
                        {
                            if (Load.Clicked)
                            {
                                if (State != EngineStates.Load)
                                {
                                    State = EngineStates.Load;
                                    LoadGame.Invoke(this, null);
                                }
                            }
                            else
                            {
                                if (Save.Clicked)
                                {
                                    if(State != EngineStates.Save)
                                    {
                                        State = EngineStates.Save;
                                        SaveGame.Invoke(this, null);
                                    }
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                    }

                    // Wenn im State Editor, vom Benutzer eingegebene Punkte zu einer Linie umrechnen
                    if((State == EngineStates.Editor)&&(Pause.Clicked == false))
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
                                    Editorline.Calculate();
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

        private double getDistance(PointF P, Line L)
        {
            Point V = new Point((int)(L.End.X - L.Start.X),(int)(L.End.Y - L.Start.Y));
            Point W = new Point((int)(P.X - L.Start.X), (int)(P.Y - L.Start.Y));

            double c1 = (double)W.X * V.X + (double)W.Y * V.Y;
            double c2 = (double)V.X * V.X + (double)V.Y * V.Y;
            double b = c1 / c2;

            double Pbx = L.Start.X + b * V.X;
            double Pby = L.Start.Y + b * V.Y;
            double Pnx = P.X - Pbx;
            double Pny = P.Y - Pby;
            return Math.Sqrt(Pnx * Pnx + Pny * Pny);
        }

        /// <summary>
        /// Distanz von einem Punkt zu einer Geraden durch zwei Punkte
        /// </summary>
        /// <param name="P">Punkt</param>
        /// <param name="L">Gerade</param>
        /// <returns>Distanz</returns>
        private double getDistanceF(PointF P, Line L)
        {
            PointF V = new PointF(L.End.X - L.Start.X, L.End.Y - L.Start.Y);
            PointF W = new PointF(P.X - L.Start.X, P.Y - L.Start.Y);

            double c1 = (double)W.X * V.X + (double)W.Y * V.Y;
            double c2 = (double)V.X * V.X + (double)V.Y * V.Y;
            double b = c1 / c2;

            double Pbx = L.Start.X + b * V.X;
            double Pby = L.Start.Y + b * V.Y;
            double Pnx = P.X - Pbx;
            double Pny = P.Y - Pby;
            return Math.Sqrt(Pnx * Pnx + Pny * Pny);
        }

        /// <summary>
        /// Prüfen ob Punkt im Bereich des rechteckigen Bereiches der Linie ist
        /// </summary>
        /// <param name="P">Punkt</param>
        /// <param name="L">Linie</param>
        /// <returns>true wenn im Bereich</returns>
        private bool inRange(PointF P, Line L)
        {
            if (L.End.X > L.Start.X)
            {
                if (((P.X+5) >= L.Start.X) && ((P.X-5) <= L.End.X))
                {
                    if (L.End.Y > L.Start.Y)
                    {
                        if (((P.Y+5) >= L.Start.Y) && ((P.Y-5) <= L.End.Y))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (((P.Y-5) <= L.Start.Y) && ((P.Y+5) >= L.End.Y))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (((P.X-5) <= L.Start.X) && ((P.X+5) >= L.End.X))
                {
                    if (L.End.Y > L.Start.Y)
                    {
                        if (((P.Y+5) >= L.Start.Y) && ((P.Y-5) <= L.End.Y))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (((P.Y-5) <= L.Start.Y) && ((P.Y+5) >= L.End.Y))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
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
