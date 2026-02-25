using System;

namespace Nexum.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public string TipoMovimiento { get; set; } // "Ingreso", "Retiro", "Transferencia"
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoPosterior { get; set; }

        // Propiedades calculadas
        public string MontoFormateado => Monto.ToString("C2");
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string Signo => TipoMovimiento == "Ingreso" ? "+" : "-";
        public string ColorTexto => TipoMovimiento == "Ingreso" ? "Green" : "Red";

        // Relación
        public virtual CuentaBancaria Cuenta { get; set; }
    }
}