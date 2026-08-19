using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BikeStore.API.Data;
using BikeStore.API.Models;

namespace BikeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicicletasController : ControllerBase
    {
        private readonly BikeStoreContext _context;

        public BicicletasController(BikeStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletas()
        {
            var bicicletas = await _context.Bicicletas
                .Include(b => b.Categoria)
                .ToListAsync();
            return Ok(bicicletas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bicicleta>> GetBicicleta(int id)
        {
            var bicicleta = await _context.Bicicletas
                .Include(b => b.Categoria)
                .FirstOrDefaultAsync(b => b.IdBicicleta == id);

            if (bicicleta == null)
                return NotFound(new { mensaje = $"No se encontró la bicicleta con Id {id}" });

            return Ok(bicicleta);
        }

        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasPorCategoria(int idCategoria)
        {
            var bicicletas = await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.IdCategoria == idCategoria)
                .ToListAsync();

            return Ok(bicicletas);
        }

        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasDisponibles()
        {
            var bicicletas = await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.Estado == "Disponible" && b.Stock > 0)
                .ToListAsync();

            return Ok(bicicletas);
        }

        [HttpGet("marca/{marca}")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasPorMarca(string marca)
        {
            var bicicletas = await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.Marca.Contains(marca))
                .ToListAsync();

            return Ok(bicicletas);
        }

        [HttpGet("stock-bajo")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasStockBajo()
        {
            const int UMBRAL_STOCK_BAJO = 5;

            var bicicletas = await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.Stock > 0 && b.Stock <= UMBRAL_STOCK_BAJO)
                .ToListAsync();

            return Ok(bicicletas);
        }

        [HttpGet("agotadas")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasAgotadas()
        {
            var bicicletas = await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.Stock == 0)
                .ToListAsync();

            return Ok(bicicletas);
        }

        [HttpPost]
        public async Task<ActionResult<Bicicleta>> PostBicicleta(Bicicleta bicicleta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == bicicleta.IdCategoria);

            if (!categoriaExiste)
                return BadRequest(new { mensaje = $"No existe la categoría con Id {bicicleta.IdCategoria}" });

            bicicleta.Estado = bicicleta.Estado ?? "Disponible";

            _context.Bicicletas.Add(bicicleta);
            await _context.SaveChangesAsync();

            var bicicletaCreada = await _context.Bicicletas
                .Include(b => b.Categoria)
                .FirstOrDefaultAsync(b => b.IdBicicleta == bicicleta.IdBicicleta);

            return CreatedAtAction(nameof(GetBicicleta), new { id = bicicleta.IdBicicleta }, bicicletaCreada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBicicleta(int id, Bicicleta bicicleta)
        {
            if (id != bicicleta.IdBicicleta)
                return BadRequest(new { mensaje = "El Id de la URL no coincide con el Id del body" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bicicletaExistente = await _context.Bicicletas.FindAsync(id);

            if (bicicletaExistente == null)
                return NotFound(new { mensaje = $"No se encontró la bicicleta con Id {id}" });

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == bicicleta.IdCategoria);

            if (!categoriaExiste)
                return BadRequest(new { mensaje = $"No existe la categoría con Id {bicicleta.IdCategoria}" });

            bicicletaExistente.IdCategoria = bicicleta.IdCategoria;
            bicicletaExistente.Marca = bicicleta.Marca;
            bicicletaExistente.Modelo = bicicleta.Modelo;
            bicicletaExistente.Precio = bicicleta.Precio;
            bicicletaExistente.Stock = bicicleta.Stock;
            bicicletaExistente.Estado = bicicleta.Estado;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Bicicletas.AnyAsync(b => b.IdBicicleta == id))
                    return NotFound(new { mensaje = $"No se encontró la bicicleta con Id {id}" });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBicicleta(int id)
        {
            var bicicleta = await _context.Bicicletas.FindAsync(id);

            if (bicicleta == null)
                return NotFound(new { mensaje = $"No se encontró la bicicleta con Id {id}" });

            _context.Bicicletas.Remove(bicicleta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
