using System.ComponentModel.DataAnnotations;

namespace BikeStore.API.DTOs
{
    // Lo que envía el cliente Web al registrar una venta
    public class CrearVentaDTO
    {
        [Required]
        public int IdCliente { get; set; }

        [Required, MinLength(1, ErrorMessage = "La venta debe tener al menos un producto")]
        public List<DetalleVentaCreateDTO> Detalles { get; set; } = new();
    }

    public class DetalleVentaCreateDTO
    {
        [Required]
        public int IdBicicleta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    // Lo que devuelve la API luego de registrar / consultar una venta
    public class VentaResponseDTO
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCliente { get; set; }
        public string? ClienteNombre { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaResponseDTO> Detalles { get; set; } = new();
    }

    public class DetalleVentaResponseDTO
    {
        public int IdBicicleta { get; set; }
        public string? BicicletaDescripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
    }
}
