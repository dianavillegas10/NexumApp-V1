using System;

namespace NexumApp.Models
{
    public class SesionActual
    {
        private static SesionActual _instancia;
        public static SesionActual Instancia => _instancia ?? (_instancia = new SesionActual());

        public int UsuarioId { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public bool EsAdmin { get; set; }
        public DateTime InicioSesion { get; set; }

        private SesionActual() { }

        public void IniciarSesion(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            UsuarioId = usuario.Id;
            Email = usuario.Email;
            NombreCompleto = usuario.NombreCompleto;
            EsAdmin = usuario.EsAdmin;
            InicioSesion = DateTime.Now;
        }

        public void CerrarSesion()
        {
            UsuarioId = 0;
            Email = null;
            NombreCompleto = null;
            EsAdmin = false;
        }

        public bool EstaLogeado => UsuarioId > 0;
    }
}