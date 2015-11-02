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
        /// Konstruktor
        /// </summary>
        public Engine()
        {
            Lines = new List<Line>();
            Rider = new Player();
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
        }

        /// <summary>
        /// Globaler Spielzustand
        /// </summary>
        public EngineStates State
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
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


            while(true)
            {
                // Berechnungen anstellen
                // Hintergrund zeichnen
                f_handle.Clear(Color.Orange);

                // Linien zeichnen
                Lines.ForEach(x=>x.Draw(f_handle, Offset, Origin));

                // Spieler zeichnen
                Rider.Draw(f_handle, Offset, Origin);

                // Spielzeitzähler 
                // Menu
                GameButtons.ForEach(x => x.Draw(f_handle));

                // Frame zeichnen
                g.DrawImage(Frame, 0, 0);
                
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
