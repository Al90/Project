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
            if (isColliding(Object.Position, Object.Size))
            {
                // originalposition behalten
                Point Original = Object.Position;

                // ball zurückplatzieren (aktuelle position kollidiert)
                Object.MoveBack();

                // länge rechnen
                double Length = Math.Sqrt(Object.Movement.X * Object.Movement.X + Object.Movement.Y * Object.Movement.Y);

                // geradenwinkel rechnen
                double Obs_Angle = 0;
                if(Vector.Y != 0)
                {
                    if(Vector.X != 0)
                    {
                        Obs_Angle = Math.Atan(Math.Abs(Vector.X) / Math.Abs(Vector.Y));
                    }
                    else
                    {
                        Obs_Angle = Math.PI / 2;
                    }
                }

                // endposition
                Object.Position.X = (int)(Original.X + Length * Math.Cos(Math.PI - Obs_Angle));
                Object.Position.Y = (int)(Original.Y + Length * Math.Sin(Math.PI - Obs_Angle));

                // umkehrwinkel
                double Ret_Angle = 0;
                if(Object.Movement.Y != 0)
                {
                    Ret_Angle = 2 * Math.PI - 2 * Obs_Angle - Math.Atan(Math.Abs(Object.Movement.X) / Math.Abs(Object.Movement.Y));
                }
                else
                {
                    Ret_Angle = 2 * Math.PI - 2 * Obs_Angle;
                }

                // umkehrbewegung
                Object.Movement.X = (int)(Length * Math.Cos(Ret_Angle));
                Object.Movement.Y = (int)(Length * Math.Sin(Ret_Angle));
            }
        }

        public Point calculateEndPoint(Point Start, ref Point End)
        {
            // schnittpunkt errechnen (linienmite)
            Point CutPoint = new Point(this.Start.X + this.Vector.X / 2, this.Start.Y + this.Vector.Y / 2);

            // vektor rechnen
            Point Vector = new Point(CutPoint.X - Start.X, CutPoint.Y - Start.Y);

            // länge rechnen
            double Length = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y);

            // linienwinkel rechnen
            double Obs_Angle = 0;
            if (this.Vector.Y == 0)
            {
                // keine bewegung in y richtung, winkel ist 0
            }
            else
            {
                Obs_Angle = Math.Atan(this.Vector.X / this.Vector.Y);
            }

            // kollisionsgeradenwinkel rechnen
            double Cos_Angle = 0;
            if (Vector.Y == 0)
            {
                // keine bewegung in y richtung, winkel ist 0
            }
            else
            {
                Cos_Angle = Math.Atan(Math.Abs(Vector.X) / Math.Abs(Vector.Y));
            }

            // halbe kollisionslänge rechnen
            double Col_Half = 0;

            Col_Half = Length * Math.Cos(Obs_Angle - Cos_Angle);

            // bewegung in x
            End.X = Start.X + (int)(2 * Col_Half * Math.Cos(Obs_Angle));

            // bewegung in y
            End.Y = Start.Y + (int)(2 * Col_Half * Math.Sin(Obs_Angle));

            return CutPoint;
        }

        public void Draw(Graphics g)
        {
            g.DrawLine(Type, Start, End);
        }

    }
}
