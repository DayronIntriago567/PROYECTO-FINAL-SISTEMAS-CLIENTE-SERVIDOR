using System.ComponentModel.DataAnnotations;

namespace BikeStore.API.Models
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int IdCliente { get; set; }
        public decimal Total { get; set; }

        public Cliente? Cliente { get; set; }
    }
}
