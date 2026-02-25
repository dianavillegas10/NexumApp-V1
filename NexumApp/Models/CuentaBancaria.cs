using Nexum.Models;
using System;
using System.Collections.Generic;

namespace Nexum.Models
{
    public class CuentaBancaria
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; } // "Corriente" o "Ahorro"
        public decimal Saldo { get; set; }
        public DateTime FechaApertura { get; set; }
        public bool Activa { get; set; }

        // Propiedades calculadas
        public string SaldoFormateado => Saldo.ToString("C2");
        public string FechaAperturaFormateada => FechaApertura.ToString("dd/MM/yyyy");

        // Relaciones
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Movimiento> Movimientos { get; set; }
        public virtual ICollection<Transferencia> TransferenciasEnviadas { get; set; }
    }
}