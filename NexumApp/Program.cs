using System;
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