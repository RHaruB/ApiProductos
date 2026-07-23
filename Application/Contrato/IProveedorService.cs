using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contrato
{
    public interface IProveedorService
    {
        public IQueryable<Models.ProveedorDTO> GetAll();
        public Task<Models.ProveedorDTO> GetById(int id);
        public Task<(int id, string mensaje)> CrearProveedor(Models.ProveedorDTO proveedor);
        public Task<(bool exito, string mensaje)> ActualizarProveedor(int id, Models.ProveedorDTO proveedor);
    }
}