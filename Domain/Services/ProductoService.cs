using Application.Contrato;
using Infrastructure.Repository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ProductoService : IProductoService
    {
        private readonly InventarioContext _context;
        public ProductoService(InventarioContext context)
        {
            _context = context;
        }
        public IQueryable<ProductoDTO> GetAll()
        {
            return _context.Productos.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            }).AsQueryable();
        }
        public async Task<ProductoDTO> GetById(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return null;
            return new ProductoDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion
            };
        }

        public async Task<(int id, string mensaje)> CrearProducto(ProductoDTO producto)
        {
            try
            {
                var nuevoProducto = new Producto
                {
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Activo = producto.Activo,
                    FechaCreacion = DateTime.Now
                };
                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();
                return (nuevoProducto.Id, "Producto creado exitosamente.");
            }
            catch (Exception ex)
            {
                return (0, $"Error al crear el producto: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> ActualizarProducto(int id, ProductoDTO producto)
        {
            try
            {
                var productoExistente = await _context.Productos.FindAsync(id);
                if (productoExistente == null)
                {
                    return (false, "Producto no encontrado.");
                }
                productoExistente.Codigo = producto.Codigo;
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Activo = producto.Activo;
                await _context.SaveChangesAsync();
                return (true, "Producto actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar el producto: {ex.Message}");
            }
        }


    }
}