using Microsoft.AspNetCore.Mvc;
 

namespace ApiProveedores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProveedorController : ControllerBase
    {


        [HttpGet("MejorTarifa/{modelo}")]
        public async Task<ActionResult<object>> get(int modelo)
        {
            string[] hoteles = new[] {
                "Hotel de la Costa","Posada del Sol","Hotel Paramount","Malibú Hotel","Motel mañanas felices","Hillside Hotel","Motel pintoresco","El viajero mundano","Hotel 4 estaciones","Radisson Hotel","Hotel Classio","Hotel Grizzly","Hotel Agoura","Hotel Crossroads","Gran Hotel","El hotel Bristol","Watson Hotel","Hotel Vitale","Hotel eleférico","El hotel Mosser","Hotel Vitale","Land’s End Resort","Palace Hotel","Four Seasons Resort","Feliz estancia","Parada rápida","Posada de los sueños","Hotel Olympus","Hotel Islas Encantadas","Hotel Diva","Hotel Elite","Hotel Zafiros","El hotel Greenwich","Hotel de la fortuna","Hotel Bliss","Hotel Triton","Dream Desert Hotel","Noche en el paraíso","Luna azul","Hotel Rose"  };


            string[] imagenes = new[] {
                "https://3.cdnpt.com/documents/LandingPageImages/20/02/06/9woeuf99sqaz6kl21c6b_or.jpg","https://d3ipii99lxarin.cloudfront.net/documents/LandingPageImages/17/08/23/jlkizsnc3al1w17gu5e3_or.jpg","https://d3ipii99lxarin.cloudfront.net/documents/LandingPageImages/17/08/23/c5s2wa3m5mddd7m9qutc_or.jpg","https://3.cdnpt.com/documents/LandingPageImages/20/02/06/9woeuf99sqaz6kl21c6b_or.jpg","https://d3ipii99lxarin.cloudfront.net/documents/LandingPageImages/17/08/23/hky1jz9tbihmr5zia5m7_or.jpg","https://d3ipii99lxarin.cloudfront.net/documents/LandingPageImages/17/08/23/5x9wzb904nssbgr22k7g_or.jpg"};

            Random rdmHabitaciones = new Random();
            Random rdmCosto = new Random();

            int habitaciones = rdmHabitaciones.Next(1, 10);

            int costo = rdmCosto.Next(500, 8000)* habitaciones;
            Boolean retornaRespuesta = false;

            DateTime fechaHora = DateTime.Now;

            retornaRespuesta = true;// fechaHora.Second % 2 == 0 ? true : false;

            

                if (retornaRespuesta)
                {

                    //Simulación de distintos modelos

                    switch (modelo)
                    {
                        case 1:
                            return Ok(new
                            {
                                habitaciones1 = habitaciones,
                                hotel1 = hoteles[Random.Shared.Next(hoteles.Length)],
                                costo1 = costo,
                                imagen1 = imagenes[Random.Shared.Next(imagenes.Length)]
                            });

                        case 2:
                            return Ok(new
                            {
                                habitaciones2 = habitaciones,
                                hotel2 = hoteles[Random.Shared.Next(hoteles.Length)],
                                costo2 = costo,
                                imagen2 = imagenes[Random.Shared.Next(imagenes.Length)]
                            });
                        case 3:

                            return Ok(new
                            {
                                habitaciones3 = habitaciones,
                                hotel3 = hoteles[Random.Shared.Next(hoteles.Length)],
                                costo3 = costo,
                                imagen3 = imagenes[Random.Shared.Next(imagenes.Length)]
                            });

                    }
                }
                return null;
            }
          
         
        } 
    
     
}









