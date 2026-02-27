using System.Collections.Generic;

namespace NexumApp.Models
{
    public class CuentaBancaria
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public decimal Saldo { get; set; }
        public System.DateTime FechaApertura { get; set; }
        public bool Activa { get; set; }

        public string SaldoFormateado => Saldo.ToString("C2");

        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Movimiento> Movimientos { get; set; }
        public virtual ICollection<Transferencia> TransferenciasEnviadas { get; set; }
    }
}