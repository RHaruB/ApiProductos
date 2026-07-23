using System;
using System.Collections.Generic;

namespace Infrastructure.Repository;

public partial class MovimientosInventario
{
    public int Id { get; set; }

    public int LoteId { get; set; }

    public int UsuarioId { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string? Observacion { get; set; }

    public virtual Lote Lote { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
