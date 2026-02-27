using System;
using System.Drawing;
using System.Windows.Forms;
using NexumApp.Models;

namespace NexumApp.Forms.Principal
{
    partial class FrmPrincipal : Form
    {
        private Label lblBienvenida;
        private Button btnCerrarSesion;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "NexumApp - Principal";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            lblBienvenida = new Label
            {
                // 👈 CORREGIDO: Usamos directamente NombreCompleto de SesionActual
                Text = SesionActual.Instancia.EstaLogeado
                    ? $"Bienvenido, {SesionActual.Instancia.NombreCompleto}"
                    : "Bienvenido",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 50)
            };

            btnCerrarSesion = new Button
            {
                Text = "Cerrar sesión",
                Location = new Point(50, 100),
                Size = new Size(120, 35)
            };
            btnCerrarSesion.Click += BtnCerrarSesion_Click;

            this.Controls.Add(lblBienvenida);
            this.Controls.Add(btnCerrarSesion);
        }

        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            SesionActual.Instancia.CerrarSesion();
            this.Hide();
            var login = new Auth.FrmLogin();
            login.FormClosed += (s, args) => Application.Exit();
            login.Show();
        }
    }
}