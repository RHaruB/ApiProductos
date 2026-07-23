using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LoteDTO
    {
        public int Id { get; set; }
        
        public DateTime? FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El ProductoId es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ProductoId debe ser mayor que 0.")]
        public int ProductoId { get; set; }

        public string? Producto { get; set; }

        [Required(ErrorMessage = "El ProveedorId es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ProveedorId debe ser mayor que 0.")]
        public int ProveedorId { get; set; }

        public string? Proveedor { get; set; }

        [Required(ErrorMessage = "El número de lote es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de lote no puede exceder los 50 caracteres.")]
        public string NumeroLote { get; set; } = null!;

        [Required(ErrorMessage = "El precio de compra es obligatorio.")]
        [Range(0.0, (double)decimal.MaxValue, ErrorMessage = "El precio de compra debe ser mayor o igual a 0.")]
        public decimal PrecioCompra { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.0, (double)decimal.MaxValue, ErrorMessage = "El precio de venta debe ser mayor o igual a 0.")]
        public decimal PrecioVenta { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0.")]
        public int Stock { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}