using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace BikeStore.Web.Controllers
{
    public class VentasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

        public VentasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET /Ventas -> Escenario 8: consultar historial de ventas
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("ventas");

            var ventas = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<Venta>>(JsonOpts) ?? new()
                : new();

            ViewBag.Clientes = await ObtenerClientesAsync();
            return View(ventas);
        }

        // GET /Ventas/PorCliente?idCliente=3 -> Escenario 9
        public async Task<IActionResult> PorCliente(int idCliente)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync($"ventas/cliente/{idCliente}");

            var ventas = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<Venta>>(JsonOpts) ?? new()
                : new();

            ViewBag.Clientes = await ObtenerClientesAsync();
            ViewBag.ClienteSeleccionado = idCliente;
            ViewBag.MensajeSinResultados = ventas.Count == 0 ? "Este cliente no tiene ventas registradas" : null;

            return View("Index", ventas);
        }

        // GET /Ventas/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await ObtenerClientesAsync();
            ViewBag.Bicicletas = await ObtenerBicicletasAsync();
            return View(new CrearVentaViewModel { Detalles = new() { new DetalleVentaCreateViewModel() } });
        }

        // POST /Ventas/Create -> Escenario 7: registrar venta con varios productos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearVentaViewModel model)
        {
            // Descarta las lineas vacias (bicicleta o cantidad en 0) antes de enviar a la API
            model.Detalles = model.Detalles.Where(d => d.IdBicicleta > 0 && d.Cantidad > 0).ToList();

            if (model.IdCliente == 0 || model.Detalles.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Selecciona un cliente y al menos una bicicleta con cantidad mayor a 0");
                ViewBag.Clientes = await ObtenerClientesAsync();
                ViewBag.Bicicletas = await ObtenerBicicletasAsync();
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.PostAsJsonAsync("ventas", model);

            if (!response.IsSuccessStatusCode)
            {
                var texto = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(texto) ? "No se pudo registrar la venta" : texto);
                ViewBag.Clientes = await ObtenerClientesAsync();
                ViewBag.Bicicletas = await ObtenerBicicletasAsync();
                return View(model);
            }

            TempData["Mensaje"] = "Venta registrada correctamente";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Cliente>> ObtenerClientesAsync()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("clientes");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<Cliente>>(JsonOpts) ?? new()
                : new();
        }

        private async Task<List<Bicicleta>> ObtenerBicicletasAsync()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("bicicletas");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<Bicicleta>>(JsonOpts) ?? new()
                : new();
        }
    }
}
