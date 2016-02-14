/////////////////////////////////////////////////////
// Projekt LineRider  // Simon Müller              //
// ET2012a            // Hard- und Softwaretechnik //
// 28.01.2016         // V1.0.0                    //
/////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineRider
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
