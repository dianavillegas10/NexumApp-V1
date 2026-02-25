using System;

namespace Nexum.Models
{
    public static class SesionActual
    {
        public static int UsuarioId { get; set; }
        public static string Email { get; set; }
        public static string NombreCompleto { get; set; }
        public static bool EsAdmin { get; set; }
        public static DateTime InicioSesion { get; set; }

        public static bool EstaLogeado => UsuarioId > 0;

        public static void IniciarSesion(Usuario usuario)
        {
            UsuarioId = usuario.Id;
            Email = usuario.Email;
            NombreCompleto = usuario.NombreCompleto;
            EsAdmin = usuario.EsAdmin;
            InicioSesion = DateTime.Now;
        }

        public static void CerrarSesion()
        {
            UsuarioId = 0;
            Email = null;
            NombreCompleto = null;
            EsAdmin = false;
        }
    }
}