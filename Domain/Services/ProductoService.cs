using Application.Contrato;
using Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ProductoService : IProductoService
    {
        private readonly InventarioContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(InventarioContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IQueryable<ProductoDTO> GetAll()
        {
            _logger.LogInformation("Consultando lista completa de productos.");
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
            _logger.LogInformation("Consultando producto con ID: {ProductoId}", id);
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                _logger.LogWarning("Producto con ID {ProductoId} no fue encontrado.", id);
                return null;
            }
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
            _logger.LogInformation("Intentando crear nuevo producto. Código: {Codigo}, Nombre: {Nombre}", producto.Codigo, producto.Nombre);
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
                
                _logger.LogInformation("Producto creado exitosamente con ID: {ProductoId}", nuevoProducto.Id);
                return (nuevoProducto.Id, "Producto creado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto con código: {Codigo}", producto.Codigo);
                return (0, $"Error al crear el producto: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> ActualizarProducto(int id, ProductoDTO producto)
        {
            _logger.LogInformation("Intentando actualizar producto con ID: {ProductoId}", id);
            try
            {
                var productoExistente = await _context.Productos.FindAsync(id);
                if (productoExistente == null)
                {
                    _logger.LogWarning("No se pudo actualizar. Producto con ID {ProductoId} no existe.", id);
                    return (false, "Producto no encontrado.");
                }
                
                productoExistente.Codigo = producto.Codigo;
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Activo = producto.Activo;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Producto con ID {ProductoId} actualizado exitosamente.", id);
                return (true, "Producto actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con ID: {ProductoId}", id);
                return (false, $"Error al actualizar el producto: {ex.Message}");
            }
        }
    }
}
