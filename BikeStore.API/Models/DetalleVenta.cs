using System.ComponentModel.DataAnnotations;

namespace BikeStore.API.Models
{
    public class DetalleVenta
    {
        [Key]
        public int IdDetalle { get; set; }
        public int IdVenta { get; set; }
        public int IdBicicleta { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }

        public Venta? Venta { get; set; }
        public Bicicleta? Bicicleta { get; set; }
    }
}
