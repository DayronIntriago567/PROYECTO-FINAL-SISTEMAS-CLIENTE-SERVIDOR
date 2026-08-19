using BikeStore.API.Data;
using BikeStore.API.DTOs;
using BikeStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.API.Controllers
{
    [ApiController]
    [Route("api/ventas")]
    public class VentasController : ControllerBase
    {
        private const decimal PORCENTAJE_IVA = 0.15m; // 15%

        private readonly BikeStoreContext _context;

        public VentasController(BikeStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaResponseDTO>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Bicicleta)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return Ok(ventas.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VentaResponseDTO>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Bicicleta)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
                return NotFound(new { mensaje = $"No existe una venta con id {id}" });

            return Ok(ToDto(venta));
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<VentaResponseDTO>>> GetVentasPorCliente(int idCliente)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.IdCliente == idCliente);
            if (!clienteExiste)
                return NotFound(new { mensaje = $"No existe un cliente con id {idCliente}" });

            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Bicicleta)
                .Where(v => v.IdCliente == idCliente)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return Ok(ventas.Select(ToDto));
        }

        [HttpPost]
        public async Task<ActionResult<VentaResponseDTO>> CrearVenta(CrearVentaDTO dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.IdCliente);
            if (cliente == null)
                return NotFound(new { mensaje = $"No existe un cliente con id {dto.IdCliente}" });

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                return BadRequest(new { mensaje = "La venta debe incluir al menos un producto" });

            var idsRepetidos = dto.Detalles.GroupBy(d => d.IdBicicleta).Any(g => g.Count() > 1);
            if (idsRepetidos)
                return BadRequest(new { mensaje = "No repitas la misma bicicleta en varias lineas, suma la cantidad en una sola" });

            await using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                var venta = new Venta
                {
                    IdCliente = dto.IdCliente,
                    Fecha = DateTime.Now,
                    Detalles = new List<DetalleVenta>()
                };

                decimal subtotalVenta = 0;

                foreach (var linea in dto.Detalles)
                {
                    var bicicleta = await _context.Bicicletas.FindAsync(linea.IdBicicleta);
                    if (bicicleta == null)
                        return NotFound(new { mensaje = $"No existe la bicicleta con id {linea.IdBicicleta}" });

                    if (bicicleta.Stock < linea.Cantidad)
                        return BadRequest(new
                        {
                            mensaje = $"Stock insuficiente para {bicicleta.Marca} {bicicleta.Modelo}. " +
                                      $"Disponible: {bicicleta.Stock}, solicitado: {linea.Cantidad}"
                        });

                    var subtotalLinea = bicicleta.Precio * linea.Cantidad;
                    subtotalVenta += subtotalLinea;

                    venta.Detalles.Add(new DetalleVenta
                    {
                        IdBicicleta = bicicleta.IdBicicleta,
                        Cantidad = linea.Cantidad,
                        Precio = bicicleta.Precio
                    });

                    bicicleta.Stock -= linea.Cantidad;
                }

                venta.Subtotal = subtotalVenta;
                venta.Iva = Math.Round(subtotalVenta * PORCENTAJE_IVA, 2);
                venta.Total = venta.Subtotal + venta.Iva;

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                var ventaCompleta = await _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Detalles!)
                        .ThenInclude(d => d.Bicicleta)
                    .FirstAsync(v => v.IdVenta == venta.IdVenta);

                return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta }, ToDto(ventaCompleta));
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                return StatusCode(500, new { mensaje = ex.Message, detalle = ex.InnerException?.Message });
            }
        }

        private static VentaResponseDTO ToDto(Venta v) => new()
        {
            IdVenta = v.IdVenta,
            Fecha = v.Fecha,
            IdCliente = v.IdCliente,
            ClienteNombre = v.Cliente != null ? $"{v.Cliente.Nombres} {v.Cliente.Apellidos}" : null,
            Subtotal = v.Subtotal,
            Iva = v.Iva,
            Total = v.Total,
            Detalles = (v.Detalles ?? new List<DetalleVenta>()).Select(d => new DetalleVentaResponseDTO
            {
                IdBicicleta = d.IdBicicleta,
                BicicletaDescripcion = d.Bicicleta != null ? $"{d.Bicicleta.Marca} {d.Bicicleta.Modelo}" : null,
                Cantidad = d.Cantidad,
                Precio = d.Precio,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }
}