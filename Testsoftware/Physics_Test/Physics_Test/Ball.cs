using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Physics_Test
{
    class Ball
    {
        public Point Position;
        public Point Movement;
        public int Size;
        private SolidBrush Picture;

        public Ball(int x, int y, int size, Color col)
        {
            Size = size;
            Position = new Point(x, y);
            Picture = new SolidBrush(col);
        }

        public void Move()
        {
            Position.X += Movement.X;
            Position.Y += Movement.Y;
        }

        public void MoveBack()
        {
            Position.X -= Movement.X;
            Position.Y -= Movement.Y;
        }

        public void Move(Point Acceleration)
        {
            Movement.X += Acceleration.X;
            Movement.Y += Acceleration.Y;
            Move();
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Picture, Position.X - Size / 2, Position.Y - Size / 2, Size, Size);
        }
    }
}
