using System;
using System.Collections.Generic;

namespace Infrastructure.Repository;

public partial class Lote
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public int ProveedorId { get; set; }

    public string NumeroLote { get; set; } = null!;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int Stock { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    public virtual Producto Producto { get; set; } = null!;

    public virtual Proveedore Proveedor { get; set; } = null!;
}
