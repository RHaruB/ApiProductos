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
        public IQueryable<ProductoDTO> GetAll();
        public Task<ProductoDTO> GetById(int id);
        public Task<(int id, string mensaje)> CrearProducto(ProductoDTO producto);
        public Task<(bool exito, string mensaje)> ActualizarProducto(int id, ProductoDTO producto);


    }
}
