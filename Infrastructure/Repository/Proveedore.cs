using System;
using System.Collections.Generic;

namespace Infrastructure.Repository;

public partial class Proveedore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
