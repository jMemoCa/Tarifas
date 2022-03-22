using Cliente.Shared.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.IServices
{
    public interface ITarifa
    {
        Task<List<TarifaDTO>> ObtenerTarifas(string tipo, int cantidad);
         
    }
}
