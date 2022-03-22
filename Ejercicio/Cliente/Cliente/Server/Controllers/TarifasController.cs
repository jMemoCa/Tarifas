using Bussiness.IServices;
using Cliente.Shared.DTOS;
using Microsoft.AspNetCore.Mvc;

namespace Cliente.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarifasController : ControllerBase
    {

        public TarifasController(ITarifa tarifa)
        {
            Tarifa = tarifa;
        }

        public ITarifa Tarifa { get; }

        [HttpGet("ObtenerTarifas/{tipo}/{cantidad}")]
        public async Task<ActionResult<List<TarifaDTO>>> Get(string tipo, int cantidad)
        {


            return await Tarifa.ObtenerTarifas(tipo,cantidad);

        }
    }
}
