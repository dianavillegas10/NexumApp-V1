using System;
using System.Windows.Forms;

namespace NexumApp
{
    /// <summary>
    /// Clase principal de la aplicación NexumApp.
    /// Contiene el punto de entrada y la configuración inicial.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación NexumApp.
        /// Configura los estilos visuales e inicia el formulario de login.
        /// </summary>
        /// <remarks>
        /// Flujo de inicio:
        /// 1. Habilita estilos visuales modernos de Windows
        /// 2. Configura el renderizado de texto compatible
        /// 3. Ejecuta el bucle de mensajes con FrmLogin como formulario inicial
        /// 
        /// Desde FrmLogin, el usuario puede:
        /// - Iniciar sesión (redirige a FrmDashboardAdmin o FrmDashboardUsuario)
        /// - Crear una cuenta nueva (abre FrmRegistro)
        /// 
        /// Requisitos previos:
        /// - XAMPP activo con MySQL corriendo
        /// - Base de datos 'nexumdb' creada con las tablas necesarias
        /// </remarks>
        [STAThread]
        static void Main()
        {
            // Habilita estilos visuales de Windows (botones, checkboxes, etc.)
            Application.EnableVisualStyles();

            // Usa GDI+ para renderizado de texto (mejor compatibilidad)
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicia la aplicación con el formulario de login
            Application.Run(new NexumApp.Forms.Auth.FrmLogin());
        }
    }
}
