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
        /// Bild des Spielers
        /// </summary>
        public Bitmap Image;
        private Font Text_Font;
        private SolidBrush Text_Brush;

        public Player(Point position, int size, Bitmap image)
        {
            Angle = 0;
            Speed = 0;
            Size = size;
            Contacted = null;
            Position = position;
            Image = image;
            Text_Font = new Font("Arial", 12f, FontStyle.Regular);
            Text_Brush = new SolidBrush(Color.Blue);
        }

        /// <summary>
        /// Zeichnen der Linie
        /// </summary>
        /// <param name="g">Graphikhandle</param>
        /// <param name="Offset">Fensteroffset</param>
        /// <param name="Origin">Koordinatennullpunkt</param>
        public void Draw(Graphics g, Point Offset, Point Origin)
        {
            g.DrawImage(Image, (int)(Offset.X + (Origin.X + Position.X) - 0.5 * Size), (int)(Offset.Y - (Origin.Y + Position.Y) - 0.5 * Size), Size, Size);
            g.DrawString(Angle.ToString() + "°", Text_Font, Text_Brush, (int)(Offset.X + (Origin.X + Position.X) + 0.5 * Size), (int)(Offset.Y - (Origin.Y + Position.Y) - 0.5 * Size));
        }
    }
}
