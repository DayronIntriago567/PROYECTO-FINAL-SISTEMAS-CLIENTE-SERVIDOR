namespace BikeStore.Web.Models
{
    // Representa lo que la API espera en el body de POST api/ventas
    public class CrearVentaViewModel
    {
        public int IdCliente { get; set; }
        public List<DetalleVentaCreateViewModel> Detalles { get; set; } = new();
    }

    public class DetalleVentaCreateViewModel
    {
        public int IdBicicleta { get; set; }
        public int Cantidad { get; set; }
    }
}
