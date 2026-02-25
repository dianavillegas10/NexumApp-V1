using System;

namespace Nexum.Models
{
    public class Transferencia
    {
        public int Id { get; set; }
        public int CuentaOrigenId { get; set; }
        public string CuentaDestino { get; set; }
        public string NombreBeneficiario { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public string Estado { get; set; } // "Completada", "Pendiente", "Fallida"

        // Propiedades calculadas
        public string MontoFormateado => Monto.ToString("C2");
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy HH:mm");

        // Relación
        public virtual CuentaBancaria CuentaOrigen { get; set; }
    }
}