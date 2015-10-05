using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Physics_Test
{
    class Obstacle
    {
        private Point Start;
        private Point End;
        private Pen Type;
        private double n;
        private Point Vector;
        
        public Obstacle(Point start, Point end, double n = 0.7f)
        {
            Start = start;
            End = end;
            Type = new Pen(new SolidBrush(Color.Black), 2);
            this.n = n;
            Vector = new Point(End.X - Start.X, End.Y - Start.Y);
        }

        /// <summary>
        /// methode, um festzustellen, ob objektposition mit hindernis kollidiert
        /// </summary>
        /// <param name="MyPosition"></param>
        /// <param name="MySize"></param>
        /// <returns></returns>
        public bool isColliding(Point MyPosition, int MySize)
        {
            // prüfen, ob durch x oder y achse geteilt wird
            if (Math.Abs(Vector.X) >= Math.Abs(Vector.Y))
            {
                // schleife durch alle x positionen, inkrement festlegen
                int Ink = Start.X < End.X ? 1 : -1;
                for (int x = 0; x < Math.Abs(Start.X - End.X); x++)
                {
                    // berechne y wert
                    int Y_Pos = (Vector.Y * 100 / Vector.X) * x * Ink / 100;

                    // neuer punkt
                    Point Test_Position = new Point(Start.X + x * Ink, Y_Pos + Start.Y);

                    // test 1: ist position auf x achse
                    if ((MyPosition.X <= (Test_Position.X + MySize)) && (MyPosition.X >= (Test_Position.X - MySize)))
                    {
                        // x - achse korrekt, prüfe y achse
                        if ((MyPosition.Y <= (Test_Position.Y + MySize)) && (MyPosition.Y >= (Test_Position.Y - MySize)))
                        {
                            // y - achse korrekt, kollision
                            return true;
                        }
                    }
                }
            }
            else
            {
                // schleife druch alle y positionen
                int Ink = Start.Y < End.Y ? 1 : -1;
                for (int y = 0; y < Math.Abs(Start.Y - End.Y); y++)
                {
                    // berechne y wert
                    int X_Pos = (Vector.X * 100 / Vector.Y) * y * Ink / 100;

                    // neuer punkt
                    Point Test_Position = new Point(X_Pos + Start.X, Start.Y + y * Ink);

                    // test 1: ist position auf y achse
                    if ((MyPosition.X <= (Test_Position.X + MySize)) && (MyPosition.X >= (Test_Position.X - MySize)))
                    {
                        // x - achse korrekt, prüfe y achse
                        if ((MyPosition.Y <= (Test_Position.Y + MySize)) && (MyPosition.Y >= (Test_Position.Y - MySize)))
                        {
                            // y - achse korrekt, kollision
                            return true;
                        }
                    }
                }
            }

            // keine kollision
            return false;
        }

        public void calculateCollision(Ball Object)
        {
            // prüfen ob kollision
            if (isColliding(Object.Position, Object.Size/2))
            {

                // endposition rechnen
                double Scalar_Product = (Object.Movement.X * Vector.X) + (Object.Movement.Y * Vector.Y);
                double Lenght_Object = Math.Sqrt(Object.Movement.X * Object.Movement.X + Object.Movement.Y * Object.Movement.Y);
                double Length_Obstacle = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y);
                double Angle = Math.Acos(Scalar_Product / (Lenght_Object * Length_Obstacle));
                Point End_Position = new Point();
                // check scalar product
                if (Scalar_Product == 0)
                {
                    // change movement direction
                    End_Position = new Point((int)(-Object.Movement.X * n), (int)(-Object.Movement.Y * n));
                }
                else
                {
                    End_Position = new Point((int)(Lenght_Object * Math.Cos(Angle) * n), (int)(Lenght_Object * Math.Sin(Angle) * n));
                }
                // bewegung umkehren
                Object.Movement = End_Position;
                // neue position rechnen
                Object.Move();
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawLine(Type, Start, End);
        }

    }
}
