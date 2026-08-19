using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;

namespace BikeStore.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("categorias");

            if (!response.IsSuccessStatusCode)
                return View(new List<Categoria>());

            var json = await response.Content.ReadAsStringAsync();
            var categorias = JsonSerializer.Deserialize<List<Categoria>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(categorias);
        }
    }
}