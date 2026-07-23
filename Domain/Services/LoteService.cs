using Application.Contrato;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
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

        public LoteService(InventarioContext context)
        {
            _context = context;
        }

        public IQueryable<LoteDTO> GetAll()
        {
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
            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null) return null;
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
            try
            {
                // Validar que exista el producto
                bool existeProducto = await _context.Productos
                    .AnyAsync(p => p.Id == lote.ProductoId && p.Activo);

                if (!existeProducto)
                    return (0, "El producto no existe o está inactivo.");

                // Validar que exista el proveedor
                bool existeProveedor = await _context.Proveedores
                    .AnyAsync(p => p.Id == lote.ProveedorId && p.Activo);

                if (!existeProveedor)
                    return (0, "El proveedor no existe o está inactivo.");

                // Validar que no exista el mismo lote
                bool existeLote = await _context.Lotes.AnyAsync(l =>
                    l.ProductoId == lote.ProductoId &&
                    l.ProveedorId == lote.ProveedorId &&
                    l.NumeroLote == lote.NumeroLote);

                if (existeLote)
                    return (0, "Ya existe un lote con ese número para el producto y proveedor seleccionados.");

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

                return (nuevoLote.Id, "Lote creado exitosamente.");
            }
            catch (Exception ex)
            {
                return (0, $"Error al crear el lote: {ex.Message}");
            }
        }
    }
}