using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LineRider
{
    public class UI_Message
    {

        public enum Clicktype
        {
            /// <summary>
            /// Linke Maustaste gedrückt
            /// </summary>
            Left,
            /// <summary>
            /// Rechte Maustaste gedrückt
            /// </summary>
            Right,
            /// <summary>
            /// Maus bewegt
            /// </summary>
            Move,
            /// <summary>
            /// Maustaste losgelassen
            /// </summary>
            Released
        };

        public Clicktype Type;

        /// <summary>
        /// Position der Maus
        /// </summary>
        public Point Position;
    }
}
