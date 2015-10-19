using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace LineRider
{
    public class Engine
    {
        /// <summary>
        /// Alle Linien
        /// </summary>
        private List<Line> Lines;
        /// <summary>
        /// Spieler
        /// </summary>
        private Player Rider;
        /// <summary>
        /// Allgemeine Beschleunigung
        /// </summary>
        private double Acceleration;
        /// <summary>
        /// Thread in dem die einzelnen Frames des Spiels gerechnet werden
        /// </summary>
        private Thread Render;
        /// <summary>
        /// Handle zur Grafik welche mit den Frames überzeichnet wird
        /// </summary>
        private Graphics g;
        /// <summary>
        /// Spieldauer
        /// </summary>
        private DateTime Playtime;

        /// <summary>
        /// Globaler Spielzustand
        /// </summary>
        public EngineStates State
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Linker Mausklick (Linke Maustaste gedrückt)
        /// </summary>
        /// <param name="Position">Position des Zeigers im Frame</param>
        public void Leftclick(Point Position)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rechter Mausklick (Rechte Maustaste gedrückt)
        /// </summary>
        /// <param name="Position">Position des Zeigers im Frame</param>
        public void Rightclick(Point Position)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Loslassen der Maustaste
        /// </summary>
        /// <param name="Position">Position des Zeigers im Frame</param>
        public void Releaseclick(Point Position)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Bewegen der gedrückten Mauscursor
        /// </summary>
        /// <param name="Position">Position des Zeigers im Frame</param>
        public void Move(Point Position)
        {
            throw new System.NotImplementedException();
        }
    }
}
