using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contrato
{
    public interface IUsuarioService
    {
        public Task<UsuarioDTO> Loggin(string usuario, string contrasena);

        Task<(int id, string mensaje)> CrearUsuario(UsuarioDTO usuarioRequest);
    }
}