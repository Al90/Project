using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Physics_Test
{
    class Cross
    {
        public Point Position;
        private Pen style;
        public int Size;

        public Color Color
        {
            get
            {
                return style.Color;
            }
            set
            {
                style = new Pen(value);
            }
        }

        public Cross(Point pos)
        {
            Position = pos;
            Color = Color.Red;
            Size = 10;
        }

        public Cross()
        {
            Position = new Point(0, 0);
            Color = Color.Red;
            Size = 10;
        }

        public void Draw(Graphics handle, int offset)
        {
            handle.DrawLine(style, Position.X - Size, offset - Position.Y, Position.X + Size, offset - Position.Y);
            handle.DrawLine(style, Position.X, offset - (Position.Y - Size), Position.X, offset - (Position.Y + Size));
        }
    }
}
