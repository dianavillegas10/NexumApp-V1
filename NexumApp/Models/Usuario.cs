<<<<<<< HEAD
using NexumApp.Models;
=======
﻿using NexumApp.Models;
>>>>>>> 0e7654f089c459908e1e4da6cea6219e81f2e458
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