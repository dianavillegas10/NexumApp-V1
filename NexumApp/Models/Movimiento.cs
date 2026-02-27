using System;

namespace NexumApp.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoPosterior { get; set; }

        public string MontoFormateado => Monto.ToString("C2");

        public virtual CuentaBancaria Cuenta { get; set; }
    }
}