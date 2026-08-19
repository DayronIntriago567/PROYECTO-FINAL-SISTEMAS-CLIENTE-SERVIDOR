using System.ComponentModel.DataAnnotations;

namespace BikeStore.API.Models
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int IdCliente { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public Cliente? Cliente { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}