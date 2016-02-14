/////////////////////////////////////////////////////
// Projekt LineRider  // Simon Müller              //
// ET2012a            // Hard- und Softwaretechnik //
// 28.01.2016         // V1.0.0                    //
/////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineRider
{
    public enum EngineStates
    {
        /// <summary>
        /// Route zeichnen
        /// </summary>
        Editor,
        /// <summary>
        /// Spiel laufen lassen
        /// </summary>
        Run,
        /// <summary>
        /// Spiel speichern
        /// </summary>
        Save,
        /// <summary>
        /// Spiel laden
        /// </summary>
        Load,
    }
}
