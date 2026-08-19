using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;

namespace BikeStore.Web.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("clientes");

            if (!response.IsSuccessStatusCode)
                return View(new List<Cliente>());

            var json = await response.Content.ReadAsStringAsync();
            var clientes = JsonSerializer.Deserialize<List<Cliente>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(clientes);
        }
    }
}