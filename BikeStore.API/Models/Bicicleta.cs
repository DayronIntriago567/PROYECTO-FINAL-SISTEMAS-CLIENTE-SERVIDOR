namespace BikeStore.API.Models
{
    public class Bicicleta
    {
        public int IdBicicleta { get; set; }
        public int IdCategoria { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; }

        public Categoria? Categoria { get; set; }
    }
}