using NexumApp.Models;
using System.Collections.Generic;

namespace NexumApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public System.DateTime? UltimoAcceso { get; set; }
        public bool EsAdmin { get; set; }
        public bool Activo { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellidos}";

        public virtual ICollection<CuentaBancaria> Cuentas { get; set; }
    }
}