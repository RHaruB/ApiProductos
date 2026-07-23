using System;
using System.Collections.Generic;

namespace Infrastructure.Repository;

public partial class Producto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
