using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(30, ErrorMessage = "El código no puede exceder los 30 caracteres.")]
        public string Codigo { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}