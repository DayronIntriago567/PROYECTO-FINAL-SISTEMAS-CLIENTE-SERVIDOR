namespace BikeStore.Web.Models
{
    public class DetalleVenta
    {
        public int IdBicicleta { get; set; }
        public string? BicicletaDescripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
    }
}