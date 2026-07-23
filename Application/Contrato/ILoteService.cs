using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contrato
{
    public interface ILoteService
    {
        IQueryable<LoteDTO> GetAll();
        Task<LoteDTO> GetById(int id);
        Task<(int id, string mensaje)> CrearLote(LoteDTO lote);
        Task<(bool exito, string mensaje)> ActualizarLote(int id, LoteDTO lote);
    }
}