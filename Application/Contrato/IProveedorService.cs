using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contrato
{
    public interface IProveedorService
    {
         IQueryable<Models.ProveedorDTO> GetAll();
         Task<Models.ProveedorDTO> GetById(int id);
         Task<(int id, string mensaje)> CrearProveedor(Models.ProveedorDTO proveedor);
         Task<(bool exito, string mensaje)> ActualizarProveedor(int id, Models.ProveedorDTO proveedor);
    }
}