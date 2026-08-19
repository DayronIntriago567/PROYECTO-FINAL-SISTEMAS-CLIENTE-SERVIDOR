using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;

namespace BikeStore.Web.Controllers
{
    public class VentasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VentasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("ventas");

            if (!response.IsSuccessStatusCode)
                return View(new List<Venta>());

            var json = await response.Content.ReadAsStringAsync();
            var ventas = JsonSerializer.Deserialize<List<Venta>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(ventas);
        }
    }
}