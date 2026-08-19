using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;

namespace BikeStore.Web.Controllers
{
    public class BicicletasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BicicletasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("bicicletas");

            if (!response.IsSuccessStatusCode)
                return View(new List<Bicicleta>());

            var json = await response.Content.ReadAsStringAsync();
            var bicicletas = JsonSerializer.Deserialize<List<Bicicleta>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(bicicletas);
        }
    }
}