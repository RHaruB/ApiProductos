using Application.Contrato;
using Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly InventarioContext _context;
        private readonly ILogger<ProveedorService> _logger;

        public ProveedorService(InventarioContext context, ILogger<ProveedorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IQueryable<ProveedorDTO> GetAll()
        {
            _logger.LogInformation("Consultando lista completa de proveedores.");
            return _context.Proveedores.Select(p => new ProveedorDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Activo = p.Activo
            }).AsQueryable();
        }

        public async Task<ProveedorDTO> GetById(int id)
        {
            _logger.LogInformation("Consultando proveedor con ID: {ProveedorId}", id);
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                _logger.LogWarning("Proveedor con ID {ProveedorId} no fue encontrado.", id);
                return null;
            }
            return new ProveedorDTO
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Activo = proveedor.Activo
            };
        }

        public async Task<(int id, string mensaje)> CrearProveedor(ProveedorDTO proveedor)
        {
            _logger.LogInformation("Intentando crear nuevo proveedor. Nombre: {Nombre}", proveedor.Nombre);
            try
            {
                var nuevoProveedor = new Proveedore
                {
                    Nombre = proveedor.Nombre,
                    Activo = proveedor.Activo
                };
                _context.Proveedores.Add(nuevoProveedor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Proveedor creado exitosamente con ID: {ProveedorId}", nuevoProveedor.Id);
                return (nuevoProveedor.Id, "Proveedor creado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el proveedor con nombre: {Nombre}", proveedor.Nombre);
                return (0, $"Error al crear el proveedor: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> ActualizarProveedor(int id, ProveedorDTO proveedor)
        {
            _logger.LogInformation("Intentando actualizar proveedor con ID: {ProveedorId}", id);
            try
            {
                var proveedorExistente = await _context.Proveedores.FindAsync(id);
                if (proveedorExistente == null)
                {
                    _logger.LogWarning("No se pudo actualizar. Proveedor con ID {ProveedorId} no existe.", id);
                    return (false, "Proveedor no encontrado.");
                }
                proveedorExistente.Nombre = proveedor.Nombre;
                proveedorExistente.Activo = proveedor.Activo;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Proveedor con ID {ProveedorId} actualizado exitosamente.", id);
                return (true, "Proveedor actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el proveedor con ID: {ProveedorId}", id);
                return (false, $"Error al actualizar el proveedor: {ex.Message}");
            }
        }
    }
}