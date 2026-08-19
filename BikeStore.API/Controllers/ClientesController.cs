using BikeStore.API.Data;
using BikeStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly BikeStoreContext _context;

        public ClientesController(BikeStoreContext context)
        {
            _context = context;
        }

        // GET: api/clientes (Leer todos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/clientes/5 (Leer por ID de cliente)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return cliente;
        }

        // GET: api/clientes/buscar/cedula/1234567890 (Búsqueda por Cédula)
        [HttpGet("buscar/cedula/{cedula}")]
        public async Task<ActionResult<Cliente>> GetClientePorCedula(string cedula)
        {
            // Adaptado a la propiedad .Cedula de tu amigo
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula);
            if (cliente == null) return NotFound("Cliente no encontrado con esa cédula.");
            return cliente;
        }

        // GET: api/clientes/buscar/apellido/Perez (Búsqueda por Apellidos)
        [HttpGet("buscar/apellido/{apellido}")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientesPorApellido(string apellido)
        {
            // Adaptado a la propiedad .Apellidos (en plural) de tu amigo
            var clientes = await _context.Clientes
                .Where(c => c.Apellidos.Contains(apellido))
                .ToListAsync();
            return clientes;
        }

        // POST: api/clientes (Crear nuevo)
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
        }

        // PUT: api/clientes/5 (Actualizar existente)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            // Adaptado a IdCliente
            if (id != cliente.IdCliente) return BadRequest();

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clientes.Any(e => e.IdCliente == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/clientes/5 (Eliminar cliente)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
