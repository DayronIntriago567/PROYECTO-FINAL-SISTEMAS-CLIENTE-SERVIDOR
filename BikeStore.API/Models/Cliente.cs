using System.ComponentModel.DataAnnotations; 

namespace BikeStore.API.Models
{
    public class Cliente
    {
        [Key] 
        public int IdCliente { get; set; }

        public string Cedula { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Correo { get; set; }
    }
}