using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace Physics_Test
{
    class Engine
    {
        private Graphics g_Handle;
        private Thread Engine_Render;
        private Rectangle Paint_Area;

        private List<Ball> Balls;
        private List<Obstacle> Lines;
        private Queue<Ball> Queued_Balls;

        private Point ArrowStart;
        private Point ArrowEnd;
        private bool ShowArrow;

        private Point TestStart;
        private Point TestEnd;
        private Point TestCut;

        public Engine(Graphics g)
        {
            g_Handle = g;

            Paint_Area = new Rectangle(0, 0, 1600, 1200);

            Balls = new List<Ball>();
            Queued_Balls = new Queue<Ball>();

            ArrowStart = new Point();
            ArrowEnd = new Point();
            ShowArrow = false;

            Lines = new List<Obstacle>();
            Lines.Add(new Obstacle(new Point(700, 700), new Point(900, 700)));
            //Lines.Add(new Obstacle(new Point(700, 700), new Point(700, 900)));

            TestStart = new Point();
            TestEnd = new Point();
            TestCut = new Point();
        }

        public void start()
        {
            // start engine
            if(g_Handle == null)
            {
                // no handle!
                return;
            }

            if((Engine_Render != null) && (Engine_Render.IsAlive == true))
            {
                // stop
                Engine_Render.Abort();
            }
            else
            {
                Engine_Render = new Thread(RenderEngine);
                Engine_Render.Name = "Engine render";
                Engine_Render.Start();
            }
        }

        private void RenderEngine(object obj)
        {
            SolidBrush Background = new SolidBrush(Color.White);

            int Ticks = Environment.TickCount;
            Point Acceleration = new Point(0, 1); // 10 px down
            Point No_Acceleration = new Point(-Acceleration.X * 2, -Acceleration.Y * 2);
            Pen Arrow = new Pen(new SolidBrush(Color.Green), 2f);
            Pen TestPoints = new Pen(new SolidBrush(Color.Red), 1f);
            Pen TestCutts = new Pen(new SolidBrush(Color.Lime), 1f);

            Bitmap RenderedFrame = new Bitmap(Paint_Area.Width, Paint_Area.Height);
            Graphics f_Handle = Graphics.FromImage(RenderedFrame);
            //Obstacle Obs = new Obstacle(new Point())

            while(true)
            {
                // draw background
                f_Handle.FillRectangle(Background, Paint_Area);

                // draw balls
                foreach(Ball ball in Balls)
                {
                    ball.Draw(f_Handle);
                }

                // draw lines
                foreach(Obstacle obs in Lines)
                {
                    obs.Draw(f_Handle);
                }

                // draw arrow
                if (ShowArrow)
                {
                    f_Handle.DrawLine(Arrow, ArrowStart, ArrowEnd);
                }

                // testpoint
                f_Handle.DrawLine(TestPoints, TestStart.X - 10, TestStart.Y, TestStart.X + 10, TestStart.Y);
                f_Handle.DrawLine(TestPoints, TestStart.X, TestStart.Y - 10, TestStart.X, TestStart.Y + 10);
                f_Handle.DrawLine(TestPoints, TestEnd.X - 10, TestEnd.Y, TestEnd.X + 10, TestEnd.Y);
                f_Handle.DrawLine(TestPoints, TestEnd.X, TestEnd.Y - 10, TestEnd.X, TestEnd.Y + 10);
                f_Handle.DrawLine(TestCutts, TestCut.X - 10, TestCut.Y, TestCut.X + 10, TestCut.Y);
                f_Handle.DrawLine(TestCutts, TestCut.X, TestCut.Y - 10, TestCut.X, TestCut.Y + 10);

                // draw on screen
                g_Handle.DrawImage(RenderedFrame, 0, 0,800,600);

                // move balls every 500 ms
                if(Environment.TickCount > (Ticks + 10))
                {
                    // refresh ticks
                    Ticks = Environment.TickCount;

                    // move balls
                    foreach(Ball ball in Balls)
                    {
                        // move ball
                        ball.Move(Acceleration);

                        // check for exit down
                        if(ball.Position.Y > Paint_Area.Height)
                        {
                            // change direction
                            ball.Movement = new Point(ball.Movement.X, -ball.Movement.Y * 70 / 100);
                            ball.Move(No_Acceleration);
                        }

                        // exit right
                        if ((ball.Position.X > Paint_Area.Width) || (ball.Position.X < 0))
                        {
                            // change direction
                            ball.Movement = new Point(-ball.Movement.X * 70 / 100, ball.Movement.Y);
                            ball.Move(No_Acceleration);
                        }

                        // check for collision
                        foreach(Obstacle obs in Lines)
                        {
                            obs.calculateCollision(ball);
                        }
                    }

                    // check for finish
                    Balls.RemoveAll(x => (x.Position.X > Paint_Area.Width) || (x.Position.X < 0) || (x.Position.Y > Paint_Area.Height) || (x.Position.Y < 0));
                }

                // check for new balls
                while(Queued_Balls.Count > 0)
                {
                    Balls.Add(Queued_Balls.Dequeue());
                }
            }
        }

        internal void stop()
        {
            if(Engine_Render != null)
            {
                Engine_Render.Abort();
            }
        }

        internal void createBall(Point point)
        {
            Queued_Balls.Enqueue(new Ball(point.X * 2, point.Y * 2, 15 * 2, Color.Red));
        }

        public void StartArrow(Point Start)
        {
            ArrowStart.X = Start.X * 2;
            ArrowStart.Y = Start.Y * 2;
            ArrowEnd.X = Start.X * 2;
            ArrowEnd.Y = Start.Y * 2;
            ShowArrow = true;
        }

        public void MoveArrow(Point Move)
        {
            ArrowEnd.X = Move.X * 2;
            ArrowEnd.Y = Move.Y * 2;
        }

        public void EndArrow()
        {
            // create new ball
            Ball toAdd = new Ball(ArrowEnd.X, ArrowEnd.Y, 15 * 2, Color.Blue);
            toAdd.Movement = new Point((ArrowEnd.X - ArrowStart.X) / 10, (ArrowEnd.Y - ArrowStart.Y) / 10);
            Queued_Balls.Enqueue(toAdd);

            ShowArrow = false;
            ArrowEnd.X = ArrowStart.X;
            ArrowEnd.Y = ArrowStart.Y;
        }

        internal void MoveStartPos(Point point)
        {
            TestStart = new Point(point.X * 2, point.Y * 2);
            TestCut = Lines[0].calculateEndPoint(TestStart, ref TestEnd);
        }
    }
}
