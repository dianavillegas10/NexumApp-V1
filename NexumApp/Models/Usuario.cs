using Nexum.Models;
using System;
using System.Collections.Generic;

namespace Nexum.Models
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
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool EsAdmin { get; set; }
        public bool Activo { get; set; }

        // Propiedad calculada
        public string NombreCompleto => $"{Nombre} {Apellidos}";

        // Relaciones: Un usuario tiene muchas cuentas
        public virtual ICollection<CuentaBancaria> Cuentas { get; set; }
    }
}