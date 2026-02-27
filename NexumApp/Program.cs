<<<<<<< HEAD
using System;
=======
﻿using System;
>>>>>>> 0e7654f089c459908e1e4da6cea6219e81f2e458
using System.Windows.Forms;
using NexumApp.Forms.Auth; // 👈 Añade este using

namespace NexumApp
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin()); // 👈 Cambiado a FrmLogin
        }
    }
}