using MySql.Data.EntityFramework;
using Nexum.Models;
using System;
using System.Data.Entity;

namespace NexumApp.Data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class NexumDbContext : DbContext
    {
        public NexumDbContext() : base("name=NexumDB")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CuentaBancaria> CuentasBancarias { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configurar relaciones igual que antes
            modelBuilder.Entity<CuentaBancaria>()
                .HasRequired(c => c.Usuario)
                .WithMany(u => u.Cuentas)
                .HasForeignKey(c => c.UsuarioId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Movimiento>()
                .HasRequired(m => m.Cuenta)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.CuentaId)
                .WillCascadeOnDelete(true);
        }
    }
}