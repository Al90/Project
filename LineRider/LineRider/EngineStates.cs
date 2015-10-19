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
