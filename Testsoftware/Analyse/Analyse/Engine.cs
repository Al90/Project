using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private Point line_start;
        public Point Line_start
        {
            get { return line_start; }
            set { line_start = value; }
        }

        private Point line_end;
        public Point Line_end
        {
            get { return line_end; }
            set { line_end = value; }
        }

        private static int CURSOR_SIZE = 5;

        Font fps_font;
        SolidBrush fps_brush;
        Pen pen_line;
        Pen pen_cursor;
        Pen pen_hline;
        double alpha;

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

            this.fps_font = new Font("Arial", 12);
            this.fps_brush = new SolidBrush(Color.Red);
            this.pen_line = new Pen(Color.Blue);
            this.pen_cursor = new Pen(Color.Green);
            this.pen_hline = new Pen(Color.Magenta);
            //this.pen_hline.DashStyle = DashStyle.Dot;
            this.pen_hline.DashPattern = new float[] { 5F, 10F };
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
            Point Offset = new Point(0, 600);

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

                // Engine 
                drawEngine(f_handle, Offset);

                // Frame zeichnen
                g_handle.DrawImage(frame, 0, 0);

                // fps inkrementieren
                frames++;
            }
        }


        /// <summary>
        /// Alle Komponenten zeichnen
        /// </summary>
        /// <param name="f_handle">Graphik Handle</param>
        /// <param name="Offset">Nullpunkt Offset</param>
        private void drawEngine(Graphics f_handle, Point Offset)
        {
            f_handle.DrawLine(pen_cursor, Offset.X + cursor.X, Offset.Y - cursor.Y + CURSOR_SIZE, Offset.X + cursor.X, Offset.Y - cursor.Y - CURSOR_SIZE);
            f_handle.DrawLine(pen_cursor, Offset.X + cursor.X - CURSOR_SIZE, Offset.Y - cursor.Y, Offset.X + cursor.X + CURSOR_SIZE, Offset.Y - cursor.Y);
            f_handle.DrawLine(pen_line, Offset.X + line_start.X, Offset.Y - line_start.Y, Offset.X + line_end.X, Offset.Y - line_end.Y);
            f_handle.DrawLine(pen_hline, Offset.X, Offset.Y - line_start.Y, 800, Offset.Y - line_start.Y);

            // Alpha berechnen
            double x = line_end.X - line_start.X;
            double y = line_end.Y - line_start.Y;
            double l = Math.Sqrt(x * x + y * y);

            // Länge auf Null prüfen
            if (l != 0)
            {
                // 1. Quadrant
                if ((x >= 0) && (y >= 0))
                {
                    alpha = Math.Acos(x / l) / Math.PI * 180f;
                }

                // 2. Quadrant
                if ((x < 0) && (y > 0))
                {
                    alpha = 180f - Math.Acos(-x / l) / Math.PI * 180f; // Spiegeln
                }

                // 3. Quadrant
                if ((x < 0) && (y < 0))
                {
                    alpha = 180f + Math.Acos(-x / l) / Math.PI * 180f;
                }

                // 3. Quadrant
                if ((x > 0) && (y < 0))
                {
                    alpha = 360f - Math.Acos(x / l) / Math.PI * 180f;
                }
            }
            else
            {
                alpha = 0;
            }


            // Alpha zeichnen
            f_handle.DrawArc(pen_line, Offset.X + line_start.X - 15, Offset.Y - line_start.Y - 15, 30f, 30f, 0f, -(float)alpha);
            f_handle.DrawString(Convert.ToString(alpha), fps_font, fps_brush, 0, 20);
        }
    }
}
