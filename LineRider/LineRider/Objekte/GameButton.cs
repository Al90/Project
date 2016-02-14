/////////////////////////////////////////////////////
// Projekt LineRider  // Simon Müller              //
// ET2012a            // Hard- und Softwaretechnik //
// 28.01.2016         // V1.0.0                    //
/////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LineRider
{
    /// <summary>
    /// Eigene Buttonklasse für das Spiel
    /// </summary>
    public class GameButton
    {
        #region Attribute

        /// <summary>
        /// Button position oben links
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

        /// <summary>
        /// Button angewählt
        /// </summary>
        public bool Selected;

        /// <summary>
        /// Button zulässig
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Button eingeschaltet
        /// </summary>
        public bool State;

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

        #endregion

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
            State = false;
            Clicked = false;
            Selected = false;
            Enabled = enabled;
            Image = image;
            HoverColor = Color.Blue;
            ClickColor = Color.LawnGreen;
        }

        /// <summary>
        /// Feststellen ob sich Position in Button befindet und wenn ja Button mit Hoverfarbe oder Klickfarbe einfärben
        /// </summary>
        public void Handle_UI(UI_Message Message)
        {
            if (Enabled && (Message != null))
            {
                switch (Message.Type)
                {
                    case UI_Message.Clicktype.Left:
                        //  Prüfen ob Zeigerposition über Button liegt
                        if (((Message.Position.X >= Position.X) && (Message.Position.X <= (Position.X + Size))) && ((Message.Position.Y >= Position.Y) && (Message.Position.Y <= (Position.Y + Size))))
                        {
                            Clicked = Message.Type == UI_Message.Clicktype.Left; // Prüfen ob gekilckt oder nicht
                        }
                        else
                        {
                            Clicked = false;
                        }
                        break;

                    case UI_Message.Clicktype.Move:

                        //  Prüfen ob Zeigerposition über Button liegt
                        if (((Message.Position.X >= Position.X) && (Message.Position.X <= (Position.X + Size))) && ((Message.Position.Y >= Position.Y) && (Message.Position.Y <= (Position.Y + Size))))
                        {
                            Selected = Message.Type == UI_Message.Clicktype.Move;
                        }
                        else
                        {
                            Selected = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Zeichnen der Linie
        /// </summary>
        /// <param name="g">Graphikhandle</param>
        public void Draw(Graphics g)
        {
            if(!Enabled)
            {
                Selected = false; 
                Clicked = false;
            }
            if (Selected || State)
            {
                if (State)
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
