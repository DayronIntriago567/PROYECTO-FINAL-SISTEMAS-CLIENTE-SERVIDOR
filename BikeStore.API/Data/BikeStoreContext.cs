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
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
    }
}