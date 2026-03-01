using NexumApp.Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NexumApp.Forms.Principal
{
    /// <summary>
    /// Dashboard principal para usuarios regulares (no administradores).
    /// Proporciona acceso a funciones bancarias personales como
    /// cuentas, transferencias, pagos y consulta de movimientos.
    /// </summary>
    /// <remarks>
    /// Este formulario se muestra cuando un usuario con EsAdmin=false inicia sesión.
    /// 
    /// Secciones del dashboard:
    /// - Header: Logo, menú de navegación y botón de cerrar sesión
    /// - Banner: Saludo personalizado con el nombre del usuario
    /// - Mis Cuentas: Panel con resumen de cuentas bancarias del usuario
    /// - Movimientos Recientes: Lista de últimas transacciones
    /// - Acceso Rápido: Botones para funciones frecuentes (transferir, pagar, etc.)
    /// - Footer: Información de copyright
    /// 
    /// NOTA: Actualmente solo tiene el diseño visual implementado.
    /// Las funcionalidades (cargar datos reales, navegación) están pendientes.
    /// </remarks>
    public partial class FrmDashboardUsuario : Form
    {
        /// <summary>
        /// Constructor del dashboard de usuario.
        /// Configura el doble buffer para evitar parpadeos.
        /// </summary>
        public FrmDashboardUsuario()
        {
            InitializeComponent();

            // Habilita doble buffer para renderizado suave
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        /// <summary>
        /// Evento de carga del formulario.
        /// Inicializa los componentes visuales y personaliza la bienvenida.
        /// </summary>
        private void FrmDashboardUsuario_Load(object sender, EventArgs e)
        {
            CargarLogo();
            PersonalizarBienvenida();
            ConfigurarHoverBotones();
        }

        /// <summary>
        /// Carga el logo de NexumApp en el header desde la carpeta Resources.
        /// </summary>
        private void CargarLogo()
        {
            try
            {
                string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "logo.png");
                if (File.Exists(rutaLogo))
                {
                    picLogo.Image = Image.FromFile(rutaLogo);
                }
            }
            catch { /* Si no existe el logo, el PictureBox quedará vacío */ }
        }

        /// <summary>
        /// Personaliza el banner de bienvenida con el nombre del usuario.
        /// Obtiene los datos del usuario desde SesionActual (singleton de sesión).
        /// </summary>
        /// <remarks>
        /// Muestra un saludo personalizado en el banner principal:
        /// "Hola, [Nombre]" con el subtítulo motivacional.
        /// </remarks>
        private void PersonalizarBienvenida()
        {
            if (SesionActual.Instancia.EstaLogeado)
            {
                var usuario = SesionActual.Instancia.Usuario;
                lblBannerTitulo.Text = $"Hola, {usuario.Nombre}";
                lblBannerSubtitulo.Text = "Tu futuro financiero al alcance.";
            }
        }

        /// <summary>
        /// Configura los efectos hover (cambio de color al pasar el ratón) 
        /// para los botones del menú y accesos rápidos.
        /// </summary>
        /// <remarks>
        /// - Botones de menú: Cambian a dorado (#D4AF37) al pasar el ratón
        /// - Botones de acceso rápido: Cambian a blanco al pasar el ratón
        /// - El botón activo (Inicio) no cambia de color
        /// </remarks>
        private void ConfigurarHoverBotones()
        {
            // Botones del menú de navegación superior
            var botonesMenu = new[] { btnInicio, btnCuentas, btnTransferencias, btnPagos, btnAyuda };

            foreach (var btn in botonesMenu)
            {
                btn.MouseEnter += (s, e) =>
                {
                    // El botón Inicio (activo) no cambia de color
                    if (btn != btnInicio)
                        btn.ForeColor = Color.FromArgb(212, 175, 55); // Dorado
                };
                btn.MouseLeave += (s, e) =>
                {
                    if (btn != btnInicio)
                        btn.ForeColor = Color.White;
                };
            }

            // Botones de acceso rápido (fondo dorado, texto oscuro)
            var botonesAcceso = new[] { btnHacerTransferencia, btnPagarFacturas, btnConsultarMovimientos, btnContactarSoporte };
            foreach (var btn in botonesAcceso)
            {
                btn.MouseEnter += (s, e) => btn.ForeColor = Color.White;
                btn.MouseLeave += (s, e) => btn.ForeColor = Color.FromArgb(50, 50, 50);
            }
        }

        /// <summary>
        /// Cierra la sesión actual y vuelve al formulario de login.
        /// Pide confirmación antes de cerrar.
        /// </summary>
        /// <remarks>
        /// Proceso:
        /// 1. Muestra diálogo de confirmación
        /// 2. Si confirma: limpia SesionActual, oculta este formulario
        /// 3. Abre FrmLogin y configura para cerrar todo al cerrar el login
        /// </remarks>
        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Estás seguro que deseas cerrar sesión?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                // Limpiar sesión global
                SesionActual.Instancia.CerrarSesion();
                this.Hide();

                // Volver al login
                var login = new Auth.FrmLogin();
                login.FormClosed += (s, args) => this.Close();
                login.Show();
            }
        }
    }
}
