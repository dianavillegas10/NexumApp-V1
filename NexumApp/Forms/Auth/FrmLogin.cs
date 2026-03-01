using NexumApp.Models;
using NexumApp.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NexumApp.Forms.Auth
{
    /// <summary>
    /// Formulario de inicio de sesión de NexumApp.
    /// Proporciona una interfaz moderna con validación en tiempo real,
    /// notificaciones animadas y sistema de bloqueo por intentos fallidos.
    /// </summary>
    /// <remarks>
    /// Características principales:
    /// - Diseño premium con panel dividido (branding + formulario)
    /// - Notificaciones tipo "toast" animadas en lugar de MessageBox
    /// - Errores inline debajo de cada campo
    /// - Bloqueo temporal después de 3 intentos fallidos
    /// - Redirección automática según rol (Admin/Usuario)
    /// - Diseño responsive que se adapta al tamaño de la ventana
    /// </remarks>
    public partial class FrmLogin : Form
    {
        /// <summary>Servicio de autenticación para validar credenciales.</summary>
        private readonly AuthService _authService = new AuthService();

        /// <summary>Referencia al formulario principal (Dashboard) que se abrirá tras el login.</summary>
        private Form _formPrincipal;

        #region Definición de colores del tema

        /// <summary>Color dorado corporativo de NexumApp.</summary>
        private readonly Color colorDorado = Color.FromArgb(212, 175, 55);

        /// <summary>Color de fondo normal para los campos de texto.</summary>
        private readonly Color colorFondoCampo = Color.FromArgb(35, 35, 40);

        /// <summary>Color de fondo cuando el campo tiene foco.</summary>
        private readonly Color colorFondoCampoFocus = Color.FromArgb(45, 45, 50);

        /// <summary>Color de fondo cuando hay error en el campo.</summary>
        private readonly Color colorFondoCampoError = Color.FromArgb(60, 35, 35);

        /// <summary>Color del borde/texto de error.</summary>
        private readonly Color colorBordeError = Color.FromArgb(255, 100, 100);

        /// <summary>Color rojo para notificaciones de error.</summary>
        private readonly Color colorNotificacionError = Color.FromArgb(220, 53, 69);

        /// <summary>Color amarillo para notificaciones de advertencia.</summary>
        private readonly Color colorNotificacionWarning = Color.FromArgb(255, 193, 7);

        /// <summary>Color azul para notificaciones informativas.</summary>
        private readonly Color colorNotificacionInfo = Color.FromArgb(23, 162, 184);

        /// <summary>Color verde para notificaciones de éxito.</summary>
        private readonly Color colorNotificacionExito = Color.FromArgb(40, 167, 69);

        #endregion

        #region Sistema de bloqueo por intentos fallidos

        /// <summary>Contador de intentos de login fallidos consecutivos.</summary>
        private int intentosFallidos = 0;

        /// <summary>Número máximo de intentos antes del bloqueo temporal.</summary>
        private const int MAX_INTENTOS = 3;

        /// <summary>Fecha/hora hasta la que el usuario está bloqueado. Null si no hay bloqueo.</summary>
        private DateTime? bloqueadoHasta = null;

        /// <summary>Duración del bloqueo temporal en segundos.</summary>
        private const int SEGUNDOS_BLOQUEO = 30;

        #endregion

        #region Sistema de animación de notificaciones

        /// <summary>Timer que controla la animación de altura del panel de notificación.</summary>
        private Timer timerAnimacion;

        /// <summary>Altura objetivo para la animación (0 = oculto, ALTURA_NOTIFICACION = visible).</summary>
        private int alturaObjetivo = 0;

        /// <summary>Altura en píxeles del panel de notificación cuando está visible.</summary>
        private const int ALTURA_NOTIFICACION = 50;

        #endregion

        /// <summary>
        /// Frases de confianza que se muestran aleatoriamente en el panel de branding.
        /// Transmiten seguridad y profesionalismo al usuario.
        /// </summary>
        private readonly string[] frasesConfianza = new string[]
        {
            "\"Tu seguridad financiera es nuestra prioridad.\"",
            "\"Más de 10 años protegiendo tu patrimonio.\"",
            "\"Tecnología de vanguardia para tu tranquilidad.\"",
            "\"Cifrado de grado bancario en cada transacción.\"",
            "\"Tu confianza, nuestro mayor activo.\""
        };

        /// <summary>
        /// Constructor del formulario de login.
        /// Configura el doble buffer para evitar parpadeos en la animación.
        /// </summary>
        public FrmLogin()
        {
            InitializeComponent();

            // Habilita doble buffer para animaciones suaves sin parpadeo
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Configura el timer de animación para el panel de notificaciones
            timerAnimacion = new Timer();
            timerAnimacion.Interval = 15; // 15ms = ~66 FPS para animación fluida
            timerAnimacion.Tick += TimerAnimacion_Tick;
        }

        /// <summary>
        /// Evento de carga del formulario.
        /// Inicializa todos los componentes visuales y configura los eventos de los campos.
        /// </summary>
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            CargarFondo();
            CargarLogo();
            MostrarFraseAleatoria();
            CentrarContenedor();
            LimpiarErrores();

            txtEmail.Focus();

            // Configura efectos visuales al entrar/salir de los campos
            txtEmail.GotFocus += (s, ev) => {
                pnlEmailContainer.BackColor = colorFondoCampoFocus;
                LimpiarErrorCampo(txtEmail);
            };
            txtEmail.LostFocus += (s, ev) => pnlEmailContainer.BackColor = colorFondoCampo;

            txtPassword.GotFocus += (s, ev) => {
                pnlPasswordContainer.BackColor = colorFondoCampoFocus;
                LimpiarErrorCampo(txtPassword);
            };
            txtPassword.LostFocus += (s, ev) => pnlPasswordContainer.BackColor = colorFondoCampo;

            // Limpia errores cuando el usuario empieza a escribir
            txtEmail.TextChanged += (s, ev) => LimpiarErrorCampo(txtEmail);
            txtPassword.TextChanged += (s, ev) => LimpiarErrorCampo(txtPassword);
        }

        /// <summary>
        /// Carga la imagen de fondo desde la carpeta Resources.
        /// </summary>
        private void CargarFondo()
        {
            try
            {
                string rutaFondo = Path.Combine(Application.StartupPath, "Resources", "background.png");
                if (File.Exists(rutaFondo))
                {
                    this.BackgroundImage = Image.FromFile(rutaFondo);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            catch { /* Si no existe el fondo, se usa el color de fondo por defecto */ }
        }

        /// <summary>
        /// Carga el logo de NexumApp desde la carpeta Resources.
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
        /// Selecciona y muestra una frase de confianza aleatoria en el slogan.
        /// </summary>
        private void MostrarFraseAleatoria()
        {
            Random rnd = new Random();
            lblSlogan.Text = frasesConfianza[rnd.Next(frasesConfianza.Length)];
        }

        /// <summary>
        /// Centra el panel contenedor principal en la ventana.
        /// Se llama al cargar y al redimensionar el formulario.
        /// </summary>
        private void CentrarContenedor()
        {
            int x = (this.ClientSize.Width - pnlContenedor.Width) / 2;
            int y = (this.ClientSize.Height - pnlContenedor.Height) / 2;
            pnlContenedor.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        /// <summary>
        /// Recentra el contenedor cuando se redimensiona la ventana.
        /// </summary>
        private void FrmLogin_Resize(object sender, EventArgs e)
        {
            CentrarContenedor();
        }

        #region Sistema de Notificaciones Moderno

        /// <summary>
        /// Tipos de notificación disponibles, cada uno con un color distintivo.
        /// </summary>
        private enum TipoNotificacion
        {
            /// <summary>Error crítico (rojo).</summary>
            Error,
            /// <summary>Advertencia o bloqueo (amarillo).</summary>
            Warning,
            /// <summary>Información neutral (azul).</summary>
            Info,
            /// <summary>Operación exitosa (verde).</summary>
            Exito
        }

        /// <summary>
        /// Muestra una notificación animada tipo "toast" en la parte superior del formulario.
        /// </summary>
        /// <param name="mensaje">Texto a mostrar en la notificación.</param>
        /// <param name="tipo">Tipo de notificación que determina el color.</param>
        /// <remarks>
        /// La notificación aparece con una animación de expansión desde altura 0.
        /// Se oculta automáticamente después de 4 segundos (controlado por timerNotificacion).
        /// </remarks>
        private void MostrarNotificacion(string mensaje, TipoNotificacion tipo = TipoNotificacion.Error)
        {
            Color colorFondo;
            Color colorTexto = Color.White;

            // Asigna colores según el tipo de notificación
            switch (tipo)
            {
                case TipoNotificacion.Warning:
                    colorFondo = colorNotificacionWarning;
                    colorTexto = Color.FromArgb(30, 30, 30); // Texto oscuro para fondo amarillo
                    break;
                case TipoNotificacion.Info:
                    colorFondo = colorNotificacionInfo;
                    break;
                case TipoNotificacion.Exito:
                    colorFondo = colorNotificacionExito;
                    break;
                default:
                    colorFondo = colorNotificacionError;
                    break;
            }

            pnlNotificacion.BackColor = colorFondo;
            lblNotificacion.ForeColor = colorTexto;
            lblNotificacion.Text = mensaje;

            // Inicia la animación de aparición
            pnlNotificacion.Visible = true;
            alturaObjetivo = ALTURA_NOTIFICACION;
            timerAnimacion.Start();

            // Reinicia el timer de auto-ocultación
            timerNotificacion.Stop();
            timerNotificacion.Start();
        }

        /// <summary>
        /// Oculta la notificación con animación de colapso.
        /// </summary>
        private void OcultarNotificacion()
        {
            alturaObjetivo = 0;
            timerAnimacion.Start();
        }

        /// <summary>
        /// Controla la animación suave del panel de notificación.
        /// Aumenta o reduce la altura gradualmente hasta alcanzar el objetivo.
        /// </summary>
        private void TimerAnimacion_Tick(object sender, EventArgs e)
        {
            int diferencia = alturaObjetivo - pnlNotificacion.Height;

            // Si estamos cerca del objetivo, finaliza la animación
            if (Math.Abs(diferencia) <= 3)
            {
                pnlNotificacion.Height = alturaObjetivo;
                timerAnimacion.Stop();

                if (alturaObjetivo == 0)
                {
                    pnlNotificacion.Visible = false;
                }
            }
            else
            {
                // Animación con efecto de desaceleración (easing)
                pnlNotificacion.Height += diferencia / 4 + (diferencia > 0 ? 1 : -1);
            }
        }

        /// <summary>
        /// Se ejecuta cuando expira el tiempo de visualización de la notificación.
        /// </summary>
        private void TimerNotificacion_Tick(object sender, EventArgs e)
        {
            timerNotificacion.Stop();
            OcultarNotificacion();
        }

        /// <summary>
        /// Muestra un mensaje de error debajo de un campo de texto específico.
        /// </summary>
        /// <param name="campo">TextBox al que corresponde el error.</param>
        /// <param name="mensaje">Mensaje de error a mostrar.</param>
        private void MostrarErrorCampo(TextBox campo, string mensaje)
        {
            Panel contenedor = campo == txtEmail ? pnlEmailContainer : pnlPasswordContainer;
            Label lblError = campo == txtEmail ? lblErrorEmail : lblErrorPassword;

            contenedor.BackColor = colorFondoCampoError;
            lblError.Text = mensaje;
            lblError.Visible = true;
        }

        /// <summary>
        /// Limpia el error de un campo específico y restaura su apariencia normal.
        /// </summary>
        /// <param name="campo">TextBox a limpiar.</param>
        private void LimpiarErrorCampo(TextBox campo)
        {
            Panel contenedor = campo == txtEmail ? pnlEmailContainer : pnlPasswordContainer;
            Label lblError = campo == txtEmail ? lblErrorEmail : lblErrorPassword;

            if (contenedor.BackColor == colorFondoCampoError)
            {
                contenedor.BackColor = campo.Focused ? colorFondoCampoFocus : colorFondoCampo;
            }
            lblError.Text = "";
            lblError.Visible = false;
        }

        /// <summary>
        /// Limpia todos los errores del formulario.
        /// </summary>
        private void LimpiarErrores()
        {
            lblErrorEmail.Text = "";
            lblErrorEmail.Visible = false;
            lblErrorPassword.Text = "";
            lblErrorPassword.Visible = false;
            pnlEmailContainer.BackColor = colorFondoCampo;
            pnlPasswordContainer.BackColor = colorFondoCampo;
        }

        #endregion

        #region Sistema de bloqueo por intentos

        /// <summary>
        /// Verifica si el usuario está actualmente bloqueado por intentos fallidos.
        /// </summary>
        /// <returns>True si está bloqueado, False si puede intentar login.</returns>
        private bool VerificarBloqueo()
        {
            if (bloqueadoHasta.HasValue)
            {
                if (DateTime.Now < bloqueadoHasta.Value)
                {
                    // Aún está bloqueado, mostrar tiempo restante
                    int segundosRestantes = (int)(bloqueadoHasta.Value - DateTime.Now).TotalSeconds;
                    MostrarNotificacion($"⏳ Cuenta bloqueada. Espera {segundosRestantes} segundos.", TipoNotificacion.Warning);
                    return true;
                }
                else
                {
                    // El bloqueo ha expirado, reiniciar contadores
                    bloqueadoHasta = null;
                    intentosFallidos = 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifica si se ha alcanzado el límite de intentos y aplica bloqueo si corresponde.
        /// </summary>
        private void VerificarIntentos()
        {
            if (intentosFallidos >= MAX_INTENTOS)
            {
                bloqueadoHasta = DateTime.Now.AddSeconds(SEGUNDOS_BLOQUEO);
                MostrarNotificacion($"🔒 Demasiados intentos. Bloqueado por {SEGUNDOS_BLOQUEO} segundos.", TipoNotificacion.Warning);
            }
        }

        #endregion

        /// <summary>
        /// Manejador del botón de login.
        /// Valida los campos, autentica con el servicio y redirige según el resultado.
        /// </summary>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LimpiarErrores();
            OcultarNotificacion();

            // Verificar si hay bloqueo activo por intentos fallidos
            if (VerificarBloqueo())
                return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            bool hayErrores = false;

            // Validación de email
            if (string.IsNullOrEmpty(email))
            {
                MostrarErrorCampo(txtEmail, "Introduce tu correo electrónico");
                hayErrores = true;
            }
            else if (!email.Contains("@") || !email.Contains("."))
            {
                MostrarErrorCampo(txtEmail, "El formato del email no es válido");
                hayErrores = true;
            }

            // Validación de contraseña
            if (string.IsNullOrEmpty(password))
            {
                MostrarErrorCampo(txtPassword, "Introduce tu contraseña");
                hayErrores = true;
            }

            if (hayErrores)
            {
                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password))
                {
                    MostrarNotificacion("Completa todos los campos para continuar", TipoNotificacion.Warning);
                }
                return;
            }

            // Feedback visual: deshabilitar botón mientras se procesa
            btnLogin.Enabled = false;
            btnLogin.Text = "VERIFICANDO...";

            try
            {
                // Llamar al servicio de autenticación
                var (usuario, resultado) = _authService.Login(email, password);

                // Procesar el resultado según el enum devuelto
                switch (resultado)
                {
                    case ResultadoLogin.Exitoso:
                        intentosFallidos = 0;
                        MostrarNotificacion("✓ Acceso correcto. Cargando...", TipoNotificacion.Exito);

                        // Timer para dar tiempo a que el usuario vea el mensaje de éxito
                        Timer timerRedireccion = new Timer();
                        timerRedireccion.Interval = 800;
                        timerRedireccion.Tick += (s, ev) =>
                        {
                            timerRedireccion.Stop();

                            // Guardar usuario en la sesión global
                            SesionActual.Instancia.IniciarSesion(usuario);
                            this.Hide();

                            // Redirección basada en rol: Admin vs Usuario normal
                            if (usuario.EsAdmin)
                            {
                                _formPrincipal = new Admin.FrmDashboardAdmin();
                            }
                            else
                            {
                                _formPrincipal = new Principal.FrmDashboardUsuario();
                            }

                            // Cerrar la aplicación cuando se cierre el dashboard
                            _formPrincipal.FormClosed += (sender2, args) => this.Close();
                            _formPrincipal.Show();
                        };
                        timerRedireccion.Start();
                        return;

                    case ResultadoLogin.UsuarioNoExiste:
                        intentosFallidos++;
                        VerificarIntentos();
                        MostrarErrorCampo(txtEmail, "Este usuario no está registrado");
                        MostrarNotificacion("Usuario no encontrado. ¿Quieres crear una cuenta?", TipoNotificacion.Error);
                        txtEmail.Focus();
                        break;

                    case ResultadoLogin.ContrasenaIncorrecta:
                        intentosFallidos++;
                        VerificarIntentos();
                        int intentosRestantes = MAX_INTENTOS - intentosFallidos;
                        MostrarErrorCampo(txtPassword, $"Contraseña incorrecta ({intentosRestantes} intentos restantes)");
                        if (intentosRestantes > 0)
                        {
                            MostrarNotificacion("Contraseña incorrecta. Verifica e intenta de nuevo.", TipoNotificacion.Error);
                        }
                        txtPassword.Clear();
                        txtPassword.Focus();
                        break;

                    case ResultadoLogin.UsuarioInactivo:
                        MostrarNotificacion("⚠ Esta cuenta está desactivada. Contacta soporte.", TipoNotificacion.Warning);
                        break;

                    case ResultadoLogin.ErrorConexion:
                        MostrarNotificacion("⚠ Error de conexión. Verifica que XAMPP esté activo.", TipoNotificacion.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MostrarNotificacion($"Error inesperado: {ex.Message}", TipoNotificacion.Error);
            }
            finally
            {
                // Restaurar el botón a su estado normal
                btnLogin.Enabled = true;
                btnLogin.Text = "ACCEDER";
            }
        }

        /// <summary>
        /// Abre el formulario de registro de nuevo usuario.
        /// </summary>
        private void LnkRegistro_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var frmRegistro = new FrmRegistro())
            {
                frmRegistro.ShowDialog();
            }
        }

        /// <summary>
        /// Muestra información de contacto para recuperación de contraseña.
        /// </summary>
        private void LnkOlvido_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MostrarNotificacion("📧 Contacta soporte: soporte@nexumbank.com | 900 123 456", TipoNotificacion.Info);
        }

        /// <summary>
        /// Cierra la aplicación.
        /// </summary>
        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
