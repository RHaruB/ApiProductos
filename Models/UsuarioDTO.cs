using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder los 50 caracteres.")]
        public string Usuario { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string PasswordHash { get; set; } = null!;

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class LogginDTO
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Usuario { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contrasena { get; set; } = null!;
    }

    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string Token { get; set; } = null!;
        public bool Activo { get; set; }
    }
}