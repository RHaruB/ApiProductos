using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contrato
{
    public interface IProductoService
    {
         IQueryable<ProductoDTO> GetAll();
         Task<ProductoDTO> GetById(int id);
         Task<(int id, string mensaje)> CrearProducto(ProductoDTO producto);
         Task<(bool exito, string mensaje)> ActualizarProducto(int id, ProductoDTO producto);


    }
}
