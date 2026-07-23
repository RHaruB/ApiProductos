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
    public class ProveedorService : IProveedorService
    {
        private readonly InventarioContext _context;
        public ProveedorService(InventarioContext context)
        {
            _context = context;
        }

        public IQueryable<ProveedorDTO> GetAll()
        {
            return _context.Proveedores.Select(p => new ProveedorDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Activo = p.Activo
            }).AsQueryable();
        }
        public async Task<ProveedorDTO> GetById(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return null;
            return new ProveedorDTO
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Activo = proveedor.Activo
            };
        }
        public async Task<(int id, string mensaje)> CrearProveedor(ProveedorDTO proveedor)
        {
            try
            {
                var nuevoProveedor = new Proveedore
                {
                    Nombre = proveedor.Nombre,
                    Activo = proveedor.Activo
                };
                _context.Proveedores.Add(nuevoProveedor);
                await _context.SaveChangesAsync();
                return (nuevoProveedor.Id, "Proveedor creado exitosamente.");
            }
            catch (Exception ex)
            {
                return (0, $"Error al crear el proveedor: {ex.Message}");
            }
        }
        public async Task<(bool exito, string mensaje)> ActualizarProveedor(int id, ProveedorDTO proveedor)
        {
            try
            {
                var proveedorExistente = await _context.Proveedores.FindAsync(id);
                if (proveedorExistente == null)
                    return (false, "Proveedor no encontrado.");
                proveedorExistente.Nombre = proveedor.Nombre;
                proveedorExistente.Activo = proveedor.Activo;
                await _context.SaveChangesAsync();
                return (true, "Proveedor actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar el proveedor: {ex.Message}");
            }
        }


    }
}