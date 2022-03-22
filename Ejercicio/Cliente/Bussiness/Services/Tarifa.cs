using Bussiness.IServices;
using Bussiness.Models;
using Cliente.Shared.DTOS;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Bussiness.Services
{
    public class Tarifa : ITarifa
    {

        public Tarifa(IHttpGet httpGet, IMemoryCache memoryCache)
        {
            HttpGet = httpGet;
            MemoryCache = memoryCache;
        }


        public IHttpGet HttpGet { get; }
        public IMemoryCache MemoryCache { get; }

        public async Task<List<TarifaDTO>> ObtenerTarifas(string tipo, int cantidad)
        {


            List<ProveedorML> proveedores = new List<ProveedorML>();
            List<TarifaDTO> tarifas = new List<TarifaDTO>();
            List<Task<RespuestaML>> TareasProveedores = new List<Task<RespuestaML>>();


            CancellationTokenSource tokenCancelacionSource = new CancellationTokenSource();

            try
            {

                CancellationToken tokenCancelacion = tokenCancelacionSource.Token;

                //Se establece el tiempo de espera cuando se cumple aborta todas las tareas.
                tokenCancelacionSource.CancelAfter(3000);

                //SimulacionProveedores esto se alimentaría de una lista en algún catálogo
                for (int i = 0; i < 100; i++)
                {

                    //Revisar si el proveedor actulmente no esta penalizado...
                    var proveedorCache = MemoryCache.Get<ProveedorML>($"Proveedor{i}");
                    if (proveedorCache == null || !proveedorCache.Penalizado)
                    {
                        //modelo permite simular que tenemos 3 posibles modelos
                        int modelo = Random.Shared.Next(1, 3);
                        proveedores.Add(new ProveedorML
                        {
                            Id = $"Proveedor{i}",
                            Penalizado = false,

                            Uri = $"https://localhost:7092/proveedor/mejorTarifa/{modelo}",
                            ReglaAsignacionML = new List<ReglaAsignacionML>()
                {
                    new ReglaAsignacionML{NombreCampoCliente="costoDTO",NombreCampoProveedor=$"costo{modelo}"},
                    new ReglaAsignacionML{NombreCampoCliente="habitacionesDTO",NombreCampoProveedor=$"habitaciones{modelo}"},
                    new ReglaAsignacionML{NombreCampoCliente="hotelDTO",NombreCampoProveedor=$"hotel{modelo}"},
                    new ReglaAsignacionML{NombreCampoCliente="imagenDTO",NombreCampoProveedor=$"imagen{modelo}"}
                }
                        });
                    }
                }


                //Itera a los proveedores, las tareas se van a ir generando según la capacidad de procesamiento...
                foreach (var proveedor in proveedores)
                {
                    TareasProveedores.Add(Task.Run(() =>
                   {
                       return (HttpGet.Get(proveedor, tokenCancelacion));

                   }));
                }

                Task.WaitAll();


                //Recaba las respuestas exitostas de los proveedores, pendiete aplicar la regla de penalización.
                foreach (var respuestaProveedor in TareasProveedores)
                {
                    var proveedor = proveedores.Where(x => x.Id == respuestaProveedor.Result.Id).FirstOrDefault();
                    if (respuestaProveedor.Result.httpStatusCode == HttpStatusCode.OK)
                    {
                        try
                        {


                            tarifas.Add(await RelacionarValores(respuestaProveedor.Result.JsonRespuesta, proveedor.ReglaAsignacionML));
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex);
                        }


                    }
                    else
                    {
                        //Evaluar si se penalizará al proveedor...
                        var proveedorCache = MemoryCache.Get<ProveedorML>(proveedor.Id);

                        if (proveedorCache != null)
                        {
                            proveedorCache.VecesSinResponder = proveedorCache.VecesSinResponder + 1;

                            proveedorCache.Penalizado = proveedorCache.VecesSinResponder >= 5 ? true : false;

                            MemoryCache.Remove(proveedor.Id);

                            //Lo dejaremos en la lista negra por x tiempo
                            MemoryCache.Set<ProveedorML>(proveedor.Id, proveedorCache, TimeSpan.FromMinutes(2));

                        }
                        else
                        {
                            //Estará en la lista por x tiempo 
                            MemoryCache.Set<ProveedorML>(proveedor.Id, proveedor, TimeSpan.FromMinutes(1));
                        }



                    }
                }

                if (tipo == "Más baratas")
                {
                    tarifas = tarifas.OrderBy(x => x.costoDTO).Take(cantidad).ToList();
                }
                else
                {
                    tarifas = tarifas.OrderByDescending(x => x.costoDTO).Take(cantidad).ToList();

                }
            }
            catch (Exception)
            {


            }
            finally
            {


                tokenCancelacionSource.Dispose();

            }
            return tarifas.ToList();


        }

        public Task<List<Cliente.Shared.DTOS.TarifaDTO>> ObtenerPeoresTarifas()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Método que permite realizar la relación del modelo devuelto con el modelo utilizado dentro de PriceTravel NOTA: se tuvo que repetir cada campo de manera fija, se intento realizar con reflection para recorrer las propiedades de la clase TarifaDTO pero falle en el intento(lo volvere a itentar).. 
        /// </summary>
        /// <param name="JsonRespuesta"></param>
        /// <param name="reglasMapeo"></param>
        /// <returns></returns>
        private async Task<TarifaDTO> RelacionarValores(string JsonRespuesta, List<ReglaAsignacionML> reglasAsignacionInformacion)
        {
            TarifaDTO tarifaDTO = new TarifaDTO();

            try
            {

                List<PropiedadML> valoresRespuesta = await ObtenerValoresJson(JsonRespuesta);


                ReglaAsignacionML reglaAsignacionML = reglasAsignacionInformacion.Where(x => x.NombreCampoCliente == "costoDTO").FirstOrDefault();

                if (reglaAsignacionML != null)
                {
                    tarifaDTO.costoDTO = Convert.ToInt32(valoresRespuesta.Where(x => x.NombreCampo == reglaAsignacionML.NombreCampoProveedor).FirstOrDefault().Valor);
                }

                reglaAsignacionML = reglasAsignacionInformacion.Where(x => x.NombreCampoCliente == "habitacionesDTO").FirstOrDefault();

                if (reglaAsignacionML != null)
                {
                    tarifaDTO.habitacionesDTO = Convert.ToInt32(valoresRespuesta.Where(x => x.NombreCampo == reglaAsignacionML.NombreCampoProveedor).FirstOrDefault().Valor);
                }
                reglaAsignacionML = reglasAsignacionInformacion.Where(x => x.NombreCampoCliente == "hotelDTO").FirstOrDefault();

                if (reglaAsignacionML != null)
                {

                    tarifaDTO.hotelDTO = valoresRespuesta.Where(x => x.NombreCampo == reglaAsignacionML.NombreCampoProveedor).FirstOrDefault().Valor ?? "";

                }

                reglaAsignacionML = reglasAsignacionInformacion.Where(x => x.NombreCampoCliente == "imagenDTO").FirstOrDefault();

                if (reglaAsignacionML != null)
                {

                    tarifaDTO.imagenDTO = valoresRespuesta.Where(x => x.NombreCampo == reglaAsignacionML.NombreCampoProveedor).FirstOrDefault().Valor ?? "";

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }

            return tarifaDTO;

        }


        #region Métodos auxiliares para complementar la funcionalidad del método RelacionarValores()

        private async Task<List<PropiedadML>> ObtenerValoresJson(string jsonstring)
        {
            var node = JsonConvert.DeserializeXNode(jsonstring, "Root");
            var elementos = await ObtenerElementosDeXDocument(node);

            return elementos;
        }

        /// <summary>
        /// se utiliza recursividad para navegar entre los elementos si es que existiera más de un nivel del servicio.
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>

        private async Task<List<PropiedadML>> ObtenerElementosDeXDocument(XDocument xElement)
        {

            List<PropiedadML> listaKeysValues = new List<PropiedadML>();

            XmlDocument doc = new XmlDocument();


            foreach (var elemento in xElement.Elements())
            {

                listaKeysValues.AddRange(await ObtenerPropiedadesDeXElment(elemento));
            }

            return listaKeysValues;
        }


        /// <summary>
        /// Permite obtener las propiedades de un elemento de un xml
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="nombrePadre"></param>
        /// <returns></returns>
        private async Task<List<PropiedadML>> ObtenerPropiedadesDeXElment(XElement xElement, string nombrePadre = "")
        {

            List<PropiedadML> listaKeysValues = new List<PropiedadML>();
            foreach (var elemento in xElement.Elements())
            {
                if (elemento.Elements().Any())
                {
                    listaKeysValues.AddRange(await ObtenerPropiedadesDeXElment(elemento, $"{nombrePadre}{elemento.Name}."));

                }
                else
                {
                    listaKeysValues.Add(new PropiedadML { NombreCampo = $"{nombrePadre}{elemento.Name}", Valor = elemento.Value.ToString() });
                }
            }

            return listaKeysValues;

        }
        #endregion

    }
}
