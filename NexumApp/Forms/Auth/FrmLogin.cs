using MySql.Data.MySqlClient;
using NexumApp.Models;
using NexumApp.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NexumApp.Forms.Auth
{
    public partial class FrmLogin : Form
    {
        private readonly AuthService _authService = new AuthService();
        private Form _formPrincipal;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)

        {
            //_authService.RegenerarTodosLosHashes();
            // Probar conexión directa (la que funciona)
            ProbarConexionDirecta();

            // Cargar logo si existe
            string logoPath = Path.Combine(Application.StartupPath, "Resources", "logo.png");
            if (File.Exists(logoPath))
            {
                try
                {
                    picLogo.Image = Image.FromFile(logoPath);
                    picLogo.BackColor = Color.Transparent;
                }
                catch { }
            }
        }

        // Método para probar la conexión directa (SIN Entity Framework)
        private void ProbarConexionDirecta()
        {
            string connString = "server=localhost;port=3306;database=nexumdb;uid=root;pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    // Verificar que hay usuarios
                    string query = "SELECT COUNT(*) FROM Usuarios";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                        string mensaje = $"✅ CONEXIÓN EXITOSA A BASE DE DATOS\n\n" +
                                       $"Base de datos: nexumdb\n" +
                                       $"Usuarios en BD: {userCount}\n" +
                                       $"Servidor: MySQL (XAMPP/WAMP)\n" +
                                       $"Estado: Conectado correctamente";

                        MessageBox.Show(mensaje, "Conexión BD - ÉXITO",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                string mensajeError = $"❌ ERROR DE CONEXIÓN A BD\n\n" +
                                    $"Mensaje: {ex.Message}\n\n" +
                                    $"POSIBLES CAUSAS:\n" +
                                    $"• XAMPP/WAMP no está iniciado\n" +
                                    $"• MySQL no está ejecutándose\n" +
                                    $"• La base de datos 'nexumdb' no existe\n" +
                                    $"• Contraseña incorrecta (pwd= vacío?)";

                MessageBox.Show(mensajeError, "Conexión BD - ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, introduce email y contraseña.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Usuario usuario = _authService.Login(email, password);

                if (usuario != null)
                {
                    SesionActual.Instancia.IniciarSesion(usuario);
                    MessageBox.Show($"Bienvenido, {usuario.NombreCompleto}.", "Login correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    _formPrincipal = new FrmHome.FrmHome1();
                    _formPrincipal.FormClosed += (s, args) => this.Close();
                    _formPrincipal.Show();
                }
                else
                {
                    MessageBox.Show("Email o contraseña incorrectos.\n\nPrueba con:\nadmin@nexum.com / 123456",
                        "Error de login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR EN LOGIN:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LnkRegistro_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("El registro se implementará cuando esté la base de datos.", "Nexum Bank - Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}