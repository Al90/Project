using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LineRider
{
    public class Line
    {
        /// <summary>
        /// Startpunkt Linie
        /// </summary>
        public Point Start;

        /// <summary>
        /// Endpunkt Linie
        /// </summary>
        public Point End;

        /// <summary>
        /// Länge der Linie
        /// </summary>
        public double Length;

        /// <summary>
        /// Winkel gegenüber der Horizontalen im Gegenuhrzeigersinn
        /// </summary>
        public double Angle;

        /// <summary>
        /// Linienstil
        /// </summary>
        private Pen Style;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Line()
        {
            Start = new Point();
            End = new Point();
            Calculate();
            Style = new Pen(Color.Black);
        }

        /// <summary>
        /// Länge und Winkel berechnen
        /// </summary>
        public void Calculate()
        {
            // Delta X ausrechnen
            int dx = Start.X - End.X;
            int dy = Start.Y - End.Y;
            // Länge berechnen
            Length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            
            // Winkel berechnen
            // Länge auf Null prüfen
            if (Length != 0)
            {
                // 1. Quadrant
                if ((dx >= 0) && (dy >= 0))
                {
                    Angle = Math.Acos(dx / Length) / Math.PI * 180f;
                }

                // 2. Quadrant
                if ((dx < 0) && (dy > 0))
                {
                    Angle = 180f - Math.Acos(-dx / Length) / Math.PI * 180f; // Spiegeln
                }

                // 3. Quadrant
                if ((dx < 0) && (dy < 0))
                {
                    Angle = 180f + Math.Acos(-dx / Length) / Math.PI * 180f;
                }

                // 3. Quadrant
                if ((dx > 0) && (dy < 0))
                {
                    Angle = 360f - Math.Acos(dx / Length) / Math.PI * 180f;
                }
            }
            else
            {
                Angle = 0;
            }

        }

        /// <summary>
        /// Zeichnen der Linie
        /// </summary>
        /// <param name="g">Graphikhandle</param>
        /// <param name="Offset">Fensteroffset</param>
        /// <param name="Origin">Koordinatennullpunkt</param>
        public void Draw(Graphics g, Point Offset, Point Origin)
        {
            g.DrawLine(Style, Offset.X + Origin.X + Start.X, Offset.Y - Origin.Y - Start.Y, Offset.X + Origin.X + End.X, Offset.Y - Origin.Y - End.Y);
        }
    }
}
