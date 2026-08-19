using Microsoft.AspNetCore.Mvc;
using BikeStore.Web.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace BikeStore.Web.Controllers
{
    public class BicicletasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

        public BicicletasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET /Bicicletas  -> Escenario 4 (consultar todas) y 5 (buscar por categoria y marca)
        public async Task<IActionResult> Index(int? idCategoria, string? marca)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");

            List<Bicicleta> bicicletas;

            if (!string.IsNullOrWhiteSpace(marca))
            {
                // La API tiene un endpoint dedicado para buscar por marca
                var response = await client.GetAsync($"bicicletas/marca/{marca}");
                bicicletas = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<List<Bicicleta>>(JsonOpts) ?? new()
                    : new();
            }
            else
            {
                var response = await client.GetAsync("bicicletas");
                bicicletas = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<List<Bicicleta>>(JsonOpts) ?? new()
                    : new();
            }

            // La categoria se filtra del lado del sitio Web (la API no combina ambos filtros a la vez)
            if (idCategoria.HasValue)
            {
                bicicletas = bicicletas.Where(b => b.IdCategoria == idCategoria.Value).ToList();
            }

            ViewBag.Categorias = await ObtenerCategoriasAsync();
            ViewBag.FiltroCategoria = idCategoria;
            ViewBag.FiltroMarca = marca;

            return View(bicicletas);
        }

        // GET /Bicicletas/StockBajo -> Escenario 10
        public async Task<IActionResult> StockBajo()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("bicicletas/stock-bajo");

            var bicicletas = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<Bicicleta>>(JsonOpts) ?? new()
                : new();

            return View(bicicletas);
        }

        // GET /Bicicletas/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = await ObtenerCategoriasAsync();
            return View(new Bicicleta());
        }

        // POST /Bicicletas/Create -> Escenario 1: registrar bicicleta (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bicicleta bicicleta)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.PostAsJsonAsync("bicicletas", bicicleta);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, await LeerErrorAsync(response));
                ViewBag.Categorias = await ObtenerCategoriasAsync();
                return View(bicicleta);
            }

            TempData["Mensaje"] = "Bicicleta registrada correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET /Bicicletas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync($"bicicletas/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var bicicleta = await response.Content.ReadFromJsonAsync<Bicicleta>(JsonOpts);
            ViewBag.Categorias = await ObtenerCategoriasAsync();
            return View(bicicleta);
        }

        // POST /Bicicletas/Edit/5 -> Escenario 2: actualizar precio y stock (PUT)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bicicleta bicicleta)
        {
            bicicleta.IdBicicleta = id;

            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.PutAsJsonAsync($"bicicletas/{id}", bicicleta);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, await LeerErrorAsync(response));
                ViewBag.Categorias = await ObtenerCategoriasAsync();
                return View(bicicleta);
            }

            TempData["Mensaje"] = "Bicicleta actualizada correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET /Bicicletas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync($"bicicletas/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var bicicleta = await response.Content.ReadFromJsonAsync<Bicicleta>(JsonOpts);
            return View(bicicleta);
        }

        // POST /Bicicletas/Delete/5 -> Escenario 3: eliminar bicicleta (DELETE)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.DeleteAsync($"bicicletas/{id}");

            TempData[response.IsSuccessStatusCode ? "Mensaje" : "Error"] =
                response.IsSuccessStatusCode ? "Bicicleta eliminada correctamente" : await LeerErrorAsync(response);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            var client = _httpClientFactory.CreateClient("BikeStoreAPI");
            var response = await client.GetAsync("categorias");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<Categoria>>(JsonOpts) ?? new()
                : new();
        }

        private static async Task<string> LeerErrorAsync(HttpResponseMessage response)
        {
            var texto = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(texto) ? $"Error del servidor ({(int)response.StatusCode})" : texto;
        }
    }
}
