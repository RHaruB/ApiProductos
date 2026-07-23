using Application.Contrato;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class LoteService: ILoteService
    {
        private readonly InventarioContext _context;
        private readonly ILogger<LoteService> _logger;

        public LoteService(InventarioContext context, ILogger<LoteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IQueryable<LoteDTO> GetAll()
        {
            _logger.LogInformation("Consultando todos los lotes.");
            return _context.Lotes
                .Select(l => new LoteDTO
                {
                    Id = l.Id,
                    ProductoId = l.ProductoId,
                    Producto = l.Producto.Nombre,

                    ProveedorId = l.ProveedorId,
                    Proveedor = l.Proveedor.Nombre,

                    NumeroLote = l.NumeroLote,
                    PrecioCompra = l.PrecioCompra,
                    PrecioVenta = l.PrecioVenta,
                    Stock = l.Stock,
                    Activo = l.Activo,
                    FechaCreacion = l.FechaIngreso
                });
        }
        
        public async Task<LoteDTO> GetById(int id)
        {
            _logger.LogInformation("Consultando lote con ID: {LoteId}", id);
            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null)
            {
                _logger.LogWarning("Lote con ID {LoteId} no encontrado.", id);
                return null;
            }
            return new LoteDTO
            {
                Id = lote.Id,
                ProductoId = lote.ProductoId,
                ProveedorId = lote.ProveedorId,
                NumeroLote = lote.NumeroLote,
                PrecioCompra = lote.PrecioCompra,
                PrecioVenta = lote.PrecioVenta,
                Stock = lote.Stock,
                Activo = lote.Activo,
                FechaCreacion = lote.FechaIngreso
            };
        }
        
        public async Task<(int id, string mensaje)> CrearLote(LoteDTO lote)
        {
            _logger.LogInformation("Intentando crear lote. ProductoId: {ProductoId}, ProveedorId: {ProveedorId}, Número de Lote: {NumeroLote}", 
                lote.ProductoId, lote.ProveedorId, lote.NumeroLote);
            try
            {
                // Validar que exista el producto
                bool existeProducto = await _context.Productos
                    .AnyAsync(p => p.Id == lote.ProductoId && p.Activo);

                if (!existeProducto)
                {
                    _logger.LogWarning("Fallo al crear lote: El producto con ID {ProductoId} no existe o está inactivo.", lote.ProductoId);
                    return (0, "El producto no existe o está inactivo.");
                }

                // Validar que exista el proveedor
                bool existeProveedor = await _context.Proveedores
                    .AnyAsync(p => p.Id == lote.ProveedorId && p.Activo);

                if (!existeProveedor)
                {
                    _logger.LogWarning("Fallo al crear lote: El proveedor con ID {ProveedorId} no existe o está inactivo.", lote.ProveedorId);
                    return (0, "El proveedor no existe o está inactivo.");
                }

                // Validar que no exista el mismo lote
                bool existeLote = await _context.Lotes.AnyAsync(l =>
                    l.ProductoId == lote.ProductoId &&
                    l.ProveedorId == lote.ProveedorId &&
                    l.NumeroLote == lote.NumeroLote);

                if (existeLote)
                {
                    _logger.LogWarning("Fallo al crear lote: Ya existe un lote con número {NumeroLote} para ProductoId {ProductoId} y ProveedorId {ProveedorId}.", 
                        lote.NumeroLote, lote.ProductoId, lote.ProveedorId);
                    return (0, "Ya existe un lote con ese número para el producto y proveedor seleccionados.");
                }

                var nuevoLote = new Lote
                {
                    ProductoId = lote.ProductoId,
                    ProveedorId = lote.ProveedorId,
                    NumeroLote = lote.NumeroLote,
                    PrecioCompra = lote.PrecioCompra,
                    PrecioVenta = lote.PrecioVenta,
                    Stock = lote.Stock,
                    Activo = lote.Activo,
                    FechaIngreso = DateTime.Now
                };

                _context.Lotes.Add(nuevoLote);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lote creado exitosamente con ID: {LoteId}", nuevoLote.Id);
                return (nuevoLote.Id, "Lote creado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el lote. ProductoId: {ProductoId}, ProveedorId: {ProveedorId}, Número de Lote: {NumeroLote}", 
                    lote.ProductoId, lote.ProveedorId, lote.NumeroLote);
                return (0, $"Error al crear el lote: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> ActualizarLote(int id, LoteDTO lote)
        {
            _logger.LogInformation("Intentando actualizar lote con ID: {LoteId}", id);
            try
            {
                var loteExistente = await _context.Lotes.FindAsync(id);
                if (loteExistente == null)
                {
                    _logger.LogWarning("No se pudo actualizar. Lote con ID {LoteId} no existe.", id);
                    return (false, "Lote no encontrado.");
                }

                // Validar que exista el producto
                bool existeProducto = await _context.Productos.AnyAsync(p => p.Id == lote.ProductoId && p.Activo);
                if (!existeProducto)
                {
                    _logger.LogWarning("Fallo al actualizar lote: El producto con ID {ProductoId} no existe o está inactivo.", lote.ProductoId);
                    return (false, "El producto seleccionado no existe o está inactivo.");
                }

                // Validar que exista el proveedor
                bool existeProveedor = await _context.Proveedores.AnyAsync(p => p.Id == lote.ProveedorId && p.Activo);
                if (!existeProveedor)
                {
                    _logger.LogWarning("Fallo al actualizar lote: El proveedor con ID {ProveedorId} no existe o está inactivo.", lote.ProveedorId);
                    return (false, "El proveedor seleccionado no existe o está inactivo.");
                }

                loteExistente.ProductoId = lote.ProductoId;
                loteExistente.ProveedorId = lote.ProveedorId;
                loteExistente.NumeroLote = lote.NumeroLote;
                loteExistente.PrecioCompra = lote.PrecioCompra;
                loteExistente.PrecioVenta = lote.PrecioVenta;
                loteExistente.Stock = lote.Stock;
                loteExistente.Activo = lote.Activo;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Lote con ID {LoteId} actualizado exitosamente.", id);
                return (true, "Lote actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el lote con ID: {LoteId}", id);
                return (false, $"Error al actualizar el lote: {ex.Message}");
            }
        }
    }
}