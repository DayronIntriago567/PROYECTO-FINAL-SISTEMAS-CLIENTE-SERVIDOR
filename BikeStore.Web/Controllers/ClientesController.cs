using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace BikeStore.Web.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

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

            var clientes = await response.Content.ReadFromJsonAsync<List<Cliente>>(JsonOpts);
            return View(clientes);
        }

        // GET /Clientes/Create
        public IActionResult Create()
        {
            return View(new Cliente());
        }

        // POST /Clientes/Create -> Escenario 6: registrar un nuevo cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.PostAsJsonAsync("clientes", cliente);

            if (!response.IsSuccessStatusCode)
            {
                var texto = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(texto) ? "No se pudo registrar el cliente" : texto);
                return View(cliente);
            }

            TempData["Mensaje"] = "Cliente registrado correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
