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
        /// Zeichnen der Linie
        /// </summary>
        /// <param name="g">Graphikhandle</param>
        /// <param name="Offset">Fensteroffset</param>
        /// <param name="Origin">Koordinatennullpunkt</param>
        public void Draw(Graphics g, Point Offset, Point Origin)
        {
            
        }
    }
}
