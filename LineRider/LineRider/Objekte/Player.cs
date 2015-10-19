using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LineRider
{
    public class Player
    {
        /// <summary>
        /// Winkel des Spielers
        /// </summary>
        private double Angle;
        /// <summary>
        /// Geschwindigkeit des Spielers
        /// </summary>
        public double Speed;
        /// <summary>
        /// Grösse des Spielers
        /// </summary>
        private int Size;
        /// <summary>
        /// Kontakt des Spielers zu einer Linie
        /// </summary>
        public Line Contacted;
        /// <summary>
        /// Position des Spielers
        /// </summary>
        public Point Position;

        /// <summary>
        /// Zeichnen der Linie
        /// </summary>
        /// <param name="g">Graphikhandle</param>
        /// <param name="Offset">Fensteroffset</param>
        /// <param name="Origin">Koordinatennullpunkt</param>
        public void Draw(Graphics g, Point Offset, Point Origin)
        {
            throw new System.NotImplementedException();
        }
    }
}
