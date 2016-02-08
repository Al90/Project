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
        private List<PointF> Deadpoints;
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
        /// Maximalgeschwindigkeit
        /// </summary>
        public static double MAX_SPEED = 100;

        /// <summary>
        /// Winkel anzeigen
        /// </summary>
        public static bool SHOW_ANGLES = false;

        /// <summary>
        /// Spielergeschwindigkeit anzeigen
        /// </summary>
        public static bool SHOW_PLAYER_SPEED = false;

        /// <summary>
        /// Geschwindigkeit der Linien anzeigen
        /// </summary>
        public static bool SHOW_LINE_SPEED = false;

        private AutoResetEvent Move;
        public AutoResetEvent Process;
        private System.Windows.Forms.Timer MoveTimer;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Engine()
        {
            Lines = new List<Line>();
            Rider = new Player(new Point(100,500),60,global::LineRider.Properties.Resources.player_gerade);
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
            Deadpoints = new List<PointF>();
            Move = new AutoResetEvent(false);
            Process = new AutoResetEvent(true);
            MoveTimer = new System.Windows.Forms.Timer();
            MoveTimer.Interval = 50;
            MoveTimer.Tick += MoveTimer_Tick;
            MoveTimer.Start();
        }

        void MoveTimer_Tick(object sender, EventArgs e)
        {
            Move.Set();
            Process.Set();
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
            int Ground = 0;
            DateTime Starttime = DateTime.Now;
            Font Time_f = new Font("Arial",24f);
            Brush Time_b = new SolidBrush(Color.Blue);
            bool Clockwise = false;
            Bitmap Stone = global::LineRider.Properties.Resources.grabstein;

            while(true)
            {
                // Auf Verarbeitungsbefehl warten
                Process.WaitOne(100);

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

                #region Berechnungen

                // Berechnungen anstellen
                if ((Move.WaitOne(0) == true) && (State == EngineStates.Run))
                {
                    // Fahrdauer rechnen
                    Playtime = DateTime.Now - Starttime;

                    // Geschwindigkeit des Spielers rechnen
                    if (Rider.Contacted == null)
                    {
                        #region Freier Fall
                        double y_speed = Rider.Speed * Math.Sin(Rider.Angle / 360 * 2 * Math.PI) - 1;
                        double x_speed = Rider.Speed * Math.Cos(Rider.Angle / 360 * 2 * Math.PI);
                        Rider.Speed = Math.Sqrt((y_speed * y_speed) + (x_speed * x_speed));
                        
                        if (Rider.Speed > MAX_SPEED)
                        {
                            double factor = MAX_SPEED / Rider.Speed;
                            y_speed = y_speed * factor;
                            x_speed = x_speed * factor;
                            Rider.Speed = MAX_SPEED;
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
                        if (Rider.Angle < 0)
                        {
                            Rider.Angle += 360;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Auf Linie
                        // Speed prüfen, falls zu klein, Spielerwinkel drehen
                        if (Rider.Speed == 0 || Math.Abs(Rider.Speed) < Rider.Contacted.acc)
                        {
                            Rider.Angle = (Rider.Angle + 180) % 360;

                            // Loopingrichtung wechseln
                            Clockwise = !Clockwise;
                        }

                        // Je nach Winkel Beschleunigung addieren
                        if (Rider.Angle > 0 && Rider.Angle < 180)
                        {
                            Rider.Speed -= Rider.Contacted.acc;
                        }
                        else
                        {
                            if (Rider.Angle != 0 && Rider.Angle != 180)
                            {
                                Rider.Speed += Rider.Contacted.acc;
                            }
                        }

                        // speed begrenzen
                        if (Rider.Speed > MAX_SPEED)
                        {
                            Rider.Speed = MAX_SPEED;
                        }
                        #endregion
                    }

                    #region Schneiden mit anderen Linien

                    // Bewegungslinie rechnen
                    PointF NextPos = getNextPosition(Rider);
                    Line Movement = new Line();
                    Movement.Start = new Point((int)Rider.Position.X, (int)Rider.Position.Y);
                    Movement.End = new Point((int)NextPos.X, (int)NextPos.Y);

                    // Schnittlinie und Schnittpunkt erstellen
                    Line CutLine = Rider.Contacted;
                    PointF CutPoint = Rider.Position;
                    double NewAngle = Clockwise ? 360 : 0;

                    // Schnittpunkt mit anderen linien prüfen
                    foreach (Line L in Lines)
                    {
                        // Eigene Linie ignorieren
                        if (L == Rider.Contacted)
                        {
                            continue;
                        }

                        // Schnittpunkt zwischen der Linie und der Bewegungslinie berechnen
                        PointF Cut = getCutPoint(Movement, L);

                        // Schnittpunkt innerhalb der Bewegung?
                        if (inRange(Cut, Movement) && inRange(Cut, L))
                        {
                            if (CutLine != null)
                            {
                                // Winkel zwischen Kontaktierter und neuer Linie Rechnen
                                double Angle = getNewAngle(CutLine.Angle, L);

                                // Winkel des Spielers?
                                if (Clockwise)
                                {
                                    if (Angle < NewAngle)
                                    {
                                        // Copy Line
                                        CutLine = L;
                                        CutPoint = Cut;
                                        // Copy Angle
                                        NewAngle = Angle;
                                    }
                                }
                                else
                                {
                                    if (Angle > NewAngle)
                                    {
                                        // Copy Line
                                        CutLine = L;
                                        CutPoint = Cut;
                                        // Copy Angle
                                        NewAngle = Angle;
                                    }
                                }
                            }
                            else
                            {
                                // Copy Line
                                CutLine = L;
                                CutPoint = Cut;
                            }
                        }
                    }

                    // Kontaktierte Linie zwischenspeichern
                    Line Contacted_Old = Rider.Contacted;

                    // Neue Kontaktierte Linie?
                    if ((CutLine != null) && (CutLine != Rider.Contacted))
                    {
                        // Kontaktierte Linie wechseln
                        Rider.Contacted = CutLine;

                        // Spieler bewegen
                        Rider.Position = CutPoint;
                    }

                    // Kontaktierte Linie vorhanden?
                    if (Rider.Contacted != null)
                    {
                        // prüfen, ob Linie ausser Reichweite
                        if (inRange(Rider.Position, Rider.Contacted) == false)
                        {
                            // Linie entfernt
                            Rider.Contacted = null;
                        }
                    }

                    #endregion

                    #region Geschwindigkeitsänderung bei neuem Kontakt mit Linien

                    // prüfen, ob sich kontaktierte linie geändert hat
                    if(Contacted_Old != Rider.Contacted)
                    {
                        // kontaktierte linie hat sich verändert, nullprüfung?
                        if(Rider.Contacted != null)
                        {
                            // linienwechsel, oder Fall auf linie, bei freiem fall auf linie, speed verkleinern
                            if (Contacted_Old == null)
                            {
                                Rider.Speed = Rider.Speed * 0.8;
                            }
                            else
                            {
                                Rider.Speed = Rider.Speed * 0.9;
                            }

                            // Neuer Spielerwinkel rechnen
                            Rider.Angle = getNewAngle(Rider.Angle, Rider.Contacted);
                        }
                        else
                        {
                            // kein Kontakt mehr
                        }
                    }

                    #endregion

                    #region Linie Färben

                    // Abfrage Linie kontaktiert
                    if (Rider.Contacted != null)
                    {
                        // Kontaktierte Linie rot färben
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

                    #endregion

                    #region Bewegung des Spielers rechnen

                    // Verschiebung in X rechnen
                    Rider.Position.X += (float)(Rider.Speed * Math.Cos(Rider.Angle * Math.PI / 180));

                    // Verschiebung in Y rechnen
                    Rider.Position.Y += (float)(Rider.Speed * Math.Sin(Rider.Angle * Math.PI / 180));

                    // Feststellen ob Spiel zu ende ist
                    if (Rider.Position.Y < Ground)
                    {
                        // Spieler Tod position merken
                        Deadpoints.Add(Rider.Position);
                        while (Deadpoints.Count > 11)
                        {
                            Deadpoints.RemoveAt(0);
                        }
                        State = EngineStates.Editor;
                        Rider.Position.X = 100;
                        Rider.Position.Y = 500;
                        Rider.Angle = 270f;
                        Rider.Speed = 0;
                        Play.Clicked = false;
                    }
                    else
                    {
                        // Verschiebung Ursprungskoordinaten
                        Origin.X = -(int)Rider.Position.X + 400;
                        Origin.Y = -(int)Rider.Position.Y + 300;
                    }

                    #endregion
                }

                #endregion

                #region Zeichnen

                // Hintergrund zeichnen
                f_handle.Clear(Color.Honeydew);

                // Linien zeichnen
                Lines.ForEach(x=>x.Draw(f_handle, Offset, Origin));

                // Editorlinie zeichnen
                if(flag_linestart)
                {
                    Editorline.Draw(f_handle, Offset, Origin);
                }

                // Grabsteine zeichnen
                foreach (PointF Pos in Deadpoints)
                {
                    f_handle.DrawImage(Stone, (int)(Offset.X + (Origin.X + Pos.X) - 0.5 * 30.55), (int)(Offset.Y - (Origin.Y + Pos.Y) - 0.5 * 36), 30.55f, 36f);
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
                            Clockwise = false;
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
                            // Verschiebung Nullpunkt auf Null
                            Origin.X = 0;
                            Origin.Y = 0;
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
                                Editorline.Start.X = Offset.X - Origin.X + Message.Position.X;
                                Editorline.Start.Y = Offset.Y - Origin.Y - Message.Position.Y;
                                Editorline.End.X = Editorline.Start.X;
                                Editorline.End.Y = Editorline.Start.Y;
                                break;

                            // Mit einem Rechtsklick wird eine Linie gelöscht
                            case UI_Message.Clicktype.Right:
                                Point Shifted = new Point(Offset.X - Origin.X + Message.Position.X, Offset.Y - Origin.Y - Message.Position.Y);
                                deleteLine(Shifted);
                                break;

                                // Mit dem bewegen der Maus wird der Endpunkt verschoben 
                            case UI_Message.Clicktype.Move:
                                if (flag_linestart)
                                {
                                    Editorline.End.X = Offset.X - Origin.X + Message.Position.X;
                                    Editorline.End.Y = Offset.Y - Origin.Y - Message.Position.Y;
                                    Editorline.Calculate();
                                }
                                break;

                                // Mit Loslassen der Linkstaste wird der Enpunkt der Linie gesetzt
                            case UI_Message.Clicktype.Released:
                                if (flag_linestart)
                                {
                                    Editorline.End.X = Offset.X - Origin.X + Message.Position.X;
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
        /// Berechnen des neuen Winkels, wenn ein Übergang zu einer neuen Linie stattfindet
        /// </summary>
        /// <param name="Angle">Aktueller Winkel</param>
        /// <param name="Contact">Neu kontaktierte Linie</param>
        /// <returns>Der neue Winkel des Spielers</returns>
        private double getNewAngle(double Angle, Line Contact)
        {
            // 1. Winkel auf Linie kippen
            double Relative_Angle = Angle - Contact.Angle;

            // 1a. Normieren auf positive Winkel
            if (Relative_Angle < 0)
            {
                Relative_Angle += 360;
            }

            // 2. Linienkontakt oben oder unten unterscheiden
            if (Relative_Angle >= 180)
            {
                // Kontakt von oben
                Relative_Angle -= 180;

                // 3. Unterscheiden zwischen links und rechts
                if (Relative_Angle < 90)
                {
                    // Bewegung nach links, Linienwinkel umkehren
                    return Contact.Angle + 180;
                }
                else
                {
                    // Bewegung nach rechts, Linienwinkel übernehmen
                    return Contact.Angle;
                }
            }
            else
            {
                // Kontakt von unten

                // 3. Unterscheiden zwischen links und rechts
                if (Relative_Angle > 90)
                {
                    // Bewegung nach links, Linienwinkel umkehren
                    return Contact.Angle + 180;
                }
                else
                {
                    // Bewegung nach rechts, Linienwinkel übernehmen
                    return Contact.Angle;
                }
            }
        }

        /// <summary>
        /// Nächste Spielerposition rechnen
        /// </summary>
        /// <param name="pl">Spieler</param>
        /// <returns>Spielerposition</returns>
        private PointF getNextPosition(Player pl)
        {
            // Neuer Punkt erstellen
            PointF Next = new Point();
            // Verschiebung in X rechnen
            Next.X = pl.Position.X + (float)(pl.Speed * Math.Cos(pl.Angle * Math.PI / 180));

            // Verschiebung in Y rechnen
            Next.Y = pl.Position.Y + (float)(pl.Speed * Math.Sin(pl.Angle * Math.PI / 180));

            // Punkt zurückgeben
            return Next;
        }

        /// <summary>
        /// Berechnen des Schnittpunkts zwischen der Linie A und B
        /// </summary>
        /// <param name="A">Linie A</param>
        /// <param name="B">Linie B</param>
        /// <returns>Schnittpunkt oder leerer Punkt</returns>
        private PointF getCutPoint(Line A, Line B)
        {
            // Schnittvariablen berechnen
            double D = ((double)A.End.X - (double)A.Start.X) * ((double)B.Start.Y - (double)B.End.Y) - ((double)B.Start.X - (double)B.End.X) * ((double)A.End.Y - (double)A.Start.Y);
            double m = (((double)B.Start.X - (double)A.Start.X) * ((double)B.Start.Y - (double)B.End.Y) - ((double)B.Start.X - (double)B.End.X) * ((double)B.Start.Y - (double)A.Start.Y)) / D;
            double n = (((double)A.End.X - (double)A.Start.X) * ((double)B.Start.X - (double)A.Start.X) - ((double)B.Start.Y - (double)A.Start.Y) * ((double)A.End.Y - (double)A.Start.Y)) / D;

            // Linien parallel
            if(D == 0)
            {
                // Parallele Linien, leerer Punkt zurückgeben
                return new PointF();
            }
            else
            {
                // Schnittpunkt auf Linie?
                if((m >= 0) && (n <= 1))
                {
                    // Schnittpunkt zurückgeben
                    return new PointF((float)((double)A.Start.X + m * ((double)A.End.X - (double)A.Start.X)), (float)((double)A.Start.Y + m * ((double)A.End.Y - (double)A.Start.Y)));
                }
                else
                {
                    // No cutpoint, return empty point
                    return new PointF();
                }
            }
        }

        /// <summary>
        /// Löschen der Linie die angeklickt wurde
        /// </summary>
        /// <param name="Click">Klickpunkt</param>
        /// <param name="Range">Abweichung</param>
        private void deleteLine(Point Click, int Range = 3)
        {
            foreach (Line L in Lines)
            {
                // Spezialfall senkrechte Linie prüfen
                if (Math.Abs(L.Start.X - L.End.X) < Range)
                {
                    #region Behandlung senkrechte Linie
                    if (L.Start.Y > L.End.Y)
                    {
                        if (((L.Start.Y + Range) > Click.Y) && ((L.End.Y - Range) < Click.Y))
                        {
                            // Linie wurde geklickt = löschen
                            Lines.Remove(L);
                            break;
                        }
                    }
                    else
                    {
                        if (((L.Start.Y - Range) < Click.Y) && ((L.End.Y + Range) > Click.Y))
                        {
                            // Linie wurde geklickt = löschen
                            Lines.Remove(L);
                            break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Klickposition auf X-Achse
                    // Prüfen ob Klickposition auf X-Achse der Linie ist
                    if (L.Start.X > L.End.X)
                    {
                        if (((L.Start.X + Range) > Click.X) && ((L.End.X - Range) < Click.X))
                        {

                        }
                        else
                        {
                            // Zur nächsten Linie wechseln
                            continue;
                        }
                    }
                    else
                    {
                        if (((L.Start.X - Range) < Click.X) && ((L.End.X + Range) > Click.X))
                        {

                        }
                        else
                        {
                            // Zur nächsten Linie wechseln
                            continue;
                        }
                    }
                    #endregion

                    // Steigung berechnen
                    double slope = (double)(L.End.Y - L.Start.Y) / (double)(L.End.X - L.Start.X);

                    // Verschiebung berechnen
                    double shift = (double)L.Start.Y - L.Start.X * slope;

                    // Klickpunkt auf Linie berechnen (Y)
                    double y_line = shift + Click.X * slope;

                    // Prüfen ob Mausklick und Klickpunkt gleich sind
                    if (((Click.Y - Range) < y_line) && ((Click.Y + Range) > y_line))
                    {
                        // Linie wurde geklickt = löschen
                        Lines.Remove(L);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Prüfen ob Punkt im Bereich des rechteckigen Bereiches der Linie ist
        /// </summary>
        /// <param name="P">Punkt</param>
        /// <param name="L">Linie</param>
        /// <param name="Range">Abweichung in Pixel</param>
        /// <returns>true wenn im Bereich</returns>
        private bool inRange(PointF P, Line L, float Range = 2f)
        {
            if(L == null)
            {
                return false;
            }
            if (L.End.X > L.Start.X)
            {
                if (((P.X + Range) >= L.Start.X) && ((P.X - Range) <= L.End.X))
                {
                    if (L.End.Y > L.Start.Y)
                    {
                        if (((P.Y + Range) >= L.Start.Y) && ((P.Y - Range) <= L.End.Y))
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
                        if (((P.Y - Range) <= L.Start.Y) && ((P.Y + Range) >= L.End.Y))
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
                if (((P.X - Range) <= L.Start.X) && ((P.X + Range) >= L.End.X))
                {
                    if (L.End.Y > L.Start.Y)
                    {
                        if (((P.Y + Range) >= L.Start.Y) && ((P.Y - Range) <= L.End.Y))
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
                        if (((P.Y - Range) <= L.Start.Y) && ((P.Y + Range) >= L.End.Y))
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
            Process.Set(); // Prozessflag setzen
        }

        public void Start(Graphics graphics)
        {
            Stop();
            g = graphics;
            Render = new Thread(RenderGraphics);
            Render.Name = "EngineRender";
            Render.Start();
        }

        public void MoveOrigin(Keys keys, int p)
        {
            if (State == EngineStates.Editor)
            {
                switch (keys)
                {
                    case Keys.Right:
                        Origin.X -= p;
                        break;

                    case Keys.Left:
                        Origin.X += p;
                        break;

                    case Keys.Up:
                        Origin.Y -= p;
                        break;

                    case Keys.Down:
                        Origin.Y += p;
                        break;

                    default:
                        break;
                }

                Process.Set(); // Prozessflag setzen
            }
        }
    }
}
