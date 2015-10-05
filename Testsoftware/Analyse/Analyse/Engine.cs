using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Analyse
{
    /// <summary>
    /// Berechnet die Frames unseres Spiels
    /// </summary>
    class Engine
    {
        private Point cursor;
        public Point Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }
        private static int CURSOR_SIZE = 5;

        /// <summary>
        /// Handler zur Grafik
        /// </summary>
        private Graphics g_handle;

        /// <summary>
        /// Rendering Prozess
        /// </summary>
        private Thread Engine_Render;

        /// <summary>
        /// Konstruktor unserer Engine
        /// </summary>
        /// <param name="g">Handle zur Originalgrafik</param>
        public Engine(Graphics g)
        {
            this.g_handle = g;
            this.cursor = new Point(0, 0);
        }

        /// <summary>
        /// Starten des Render
        /// </summary>
        public void Start()
        {
            if (g_handle != null) // Kontrollieren ob Grafikhandle verfügbar ist
            {
                Stop(); // Falls Render aktiv, Render stoppen
                Engine_Render = new Thread(render); // Neuer Thread erzeugen
                Engine_Render.Name = "Enginerender"; // Name geben
                Engine_Render.Start(); // Thread starten
            }
        }

        /// <summary>
        /// Stoppen des Render
        /// </summary>
        public void Stop()
        {
            if ((Engine_Render != null) && (Engine_Render.IsAlive == true)) // Prüfen ob Renderobjekt existiert und läuft
            {
                Engine_Render.Abort(); // Thread stoppen
            }
        }

        /// <summary>
        /// Renderfunktion
        /// </summary>
        /// <param name="obj"></param>
        private void render(object obj)
        {
            int frames = 0;
            int fps = 0;
            int ticks = System.Environment.TickCount;




            Bitmap frame = new Bitmap(800, 600);
            Graphics f_handle = Graphics.FromImage(frame);
            Font fps_font = new Font("Arial",12);
            SolidBrush fps_brush = new SolidBrush(Color.Red);
            Pen pen_line = new Pen(Color.Blue);
            Pen pen_cursor = new Pen(Color.Green);

            while(true)
            {
                // 1 Sekundentakt
                if((Environment.TickCount - ticks) >1000)
                {
                    ticks = System.Environment.TickCount;
                    fps = frames;
                    frames = 0;
                }

                // Hintergrund 
                f_handle.Clear(Color.White);

                // FPS anzeigen
                f_handle.DrawString(fps.ToString(), fps_font,fps_brush,0,0);

                // Cursor zeichnen
                f_handle.DrawLine(pen_cursor, cursor.X, cursor.Y-CURSOR_SIZE, cursor.X, cursor.Y+CURSOR_SIZE);
                f_handle.DrawLine(pen_cursor, cursor.X-CURSOR_SIZE, cursor.Y, cursor.X+CURSOR_SIZE, cursor.Y);

                // Engine zeichnen
                f_handle.DrawLine(pen_line, 100, 100, 200, 100);

                // Frame zeichnen
                g_handle.DrawImage(frame, 0, 0);

                // fps inkrementieren
                frames++;
            }
        }
    }
}
