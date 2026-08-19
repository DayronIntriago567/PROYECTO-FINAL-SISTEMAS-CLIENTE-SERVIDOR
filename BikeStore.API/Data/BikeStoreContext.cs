using BikeStore.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.API.Data
{
    public class BikeStoreContext : DbContext
    {
        public BikeStoreContext(DbContextOptions<BikeStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Bicicleta> Bicicletas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>().ToTable("Categoria").HasKey(c => c.IdCategoria);
            modelBuilder.Entity<Bicicleta>().ToTable("Bicicleta").HasKey(b => b.IdBicicleta);
            modelBuilder.Entity<Cliente>().ToTable("Cliente").HasKey(c => c.IdCliente);
            modelBuilder.Entity<Venta>().ToTable("Venta").HasKey(v => v.IdVenta);
            modelBuilder.Entity<DetalleVenta>().ToTable("DetalleVenta").HasKey(d => d.IdDetalle);

            modelBuilder.Entity<Bicicleta>()
                .HasOne(b => b.Categoria)
                .WithMany()
                .HasForeignKey(b => b.IdCategoria);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.IdCliente);

            modelBuilder.Entity<Venta>()
                .HasMany(v => v.Detalles)
                .WithOne()
                .HasForeignKey(d => d.IdVenta);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Bicicleta)
                .WithMany()
                .HasForeignKey(d => d.IdBicicleta);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.Subtotal)
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}