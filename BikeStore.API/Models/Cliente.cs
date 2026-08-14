namespace BikeStore.API.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
    }
}