using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Physics_Test
{
    class Line
    {
        public Point Start;
        public Point End;
        public Point Vector
        {
            get
            {
                return new Point(End.X - Start.X, End.Y - Start.Y);
            }
        }
        public double Lenght
        {
            get
            {
                return Math.Sqrt(Math.Pow(Vector.X, 2) + Math.Pow(Vector.Y, 2));
            }
        }

        public double Angle
        {
            get
            {
                // 1. quadrant
                if((Vector.X >= 0) && (Vector.Y >= 0))
                {
                    Quad = 1;
                    return Lenght != 0 ? Math.Asin(Vector.Y / Lenght) / Math.PI * 180 : 0;
                }

                // 2. quadrant
                if ((Vector.X < 0) && (Vector.Y >= 0))
                {
                    Quad = 2;
                    return Lenght != 0 ? (Math.PI - Math.Asin(Vector.Y / Lenght)) / Math.PI * 180 : 0;
                }

                // 3. quadrant
                if ((Vector.X < 0) && (Vector.Y < 0))
                {
                    Quad = 3;
                    return Lenght != 0 ? Math.Asin(-Vector.Y / Lenght) / Math.PI * 180 + 180: 0;
                }

                // 4. quadrant
                if ((Vector.X > 0) && (Vector.Y < 0))
                {
                    Quad = 4;
                    return Lenght != 0 ? Math.Asin(Vector.Y / Lenght) / Math.PI * 180 + 360: 0;
                }

                Quad = 0;
                return 0;
            }
        }

        private int Quad;


        private Pen style;
        private Font font;
        private SolidBrush text;

        public Color Color
        {
            get
            {
                return style.Color;
            }
            set
            {
                style = new Pen(value);
                text = new SolidBrush(value);
            }
        }

        public Line()
        {
            Color = Color.Blue;
            font = new Font("Arial", 12);
            text = new SolidBrush(Color);
        }

        public void Draw(Graphics handle, int offset)
        {
            handle.DrawLine(style, Start.X, offset - Start.Y, End.X, offset - End.Y);
            handle.DrawArc(style, Start.X - 50, offset - Start.Y - 50, 100, 100, 0f, -(float)Angle);
            handle.DrawString("Q" + Quad.ToString() + "\n" + Math.Round(Angle, 1).ToString() + "°\nl:" + Math.Round(Lenght, 1).ToString(), font, text, Start.X, offset - Start.Y);
        }
    }
}
