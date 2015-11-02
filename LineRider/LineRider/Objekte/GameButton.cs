using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LineRider
{
    public class GameButton
    {
        /// <summary>
        /// Button position
        /// </summary>
        public Point Position;
        /// <summary>
        /// Button grösse
        /// </summary>
        public int Size;
        /// <summary>
        /// Status des Button
        /// </summary>
        public bool Clicked;

        public bool Selected;
        /// <summary>
        /// Button zulässig
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// Bild des Button
        /// </summary>
        public Bitmap Image;
        /// <summary>
        /// Farbe in welcher der Butten erscheint wenn sich der Mauscursor über dem Button befindet
        /// </summary>
        public Color HoverColor;
        /// <summary>
        /// Farbe in welcher der Butten erscheint wenn der Button geklickt ist
        /// </summary>
        public Color ClickColor;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="position">Position des Button</param>
        /// <param name="size">Grösse des Button</param>
        /// <param name="enabled">Button zulässig</param>
        /// <param name="image">Bild des Button</param>
        public GameButton(Point position, int size, bool enabled, Bitmap image)
        {
            Position = position;
            Size = size;
            Clicked = false;
            Selected = false;
            Enabled = enabled;
            Image = image;
            HoverColor = Color.Blue;
            ClickColor = Color.LawnGreen;
        }

        /// <summary>
        /// Feststellen ob sich Position in Button befindet und wenn ja Button mit Klickfarbe einfärben
        /// </summary>
        /// <param name="Position">Klickposition des Mauscursors</param>
        public void Click(Point Position)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Feststellen ob sich Position in Button befindet und wenn ja Button mit Hoverfarbe einfärben
        /// </summary>
        public void Hover()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Zeichnen der Linie
        /// </summary>
        /// <param name="g">Graphikhandle</param>
        public void Draw(Graphics g)
        {
            if (Selected || Clicked)
            {
                if (Clicked)
                {
                    g.FillEllipse(new SolidBrush(ClickColor), Position.X, Position.Y, Size, Size);
                }
                else
                {
                    g.FillEllipse(new SolidBrush(HoverColor), Position.X, Position.Y, Size, Size);
                }
            }
            g.DrawImage(Image, Position.X + 5, Position.Y + 5, Size - 10, Size - 10);
        }
    }
}
