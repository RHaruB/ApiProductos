using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LoteDTO
    {
        public int Id { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int ProductoId { get; set; }

        public string Producto { get; set; }
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public string NumeroLote { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}