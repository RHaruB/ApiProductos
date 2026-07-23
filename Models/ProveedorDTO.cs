using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProveedorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }
}