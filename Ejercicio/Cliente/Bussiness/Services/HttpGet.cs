using Bussiness.IServices;
using Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bussiness.Services
{
    public class HttpGet : IHttpGet
    {

        public async Task<RespuestaML> Get(ProveedorML proveedorML, CancellationToken tokenCancelacion)
        {
            RespuestaML respuestaML = new RespuestaML();
            try
            {
                respuestaML.Id = proveedorML.Id;
                //Utilizare HttpClient pero para algo más optimizado conviene utilizar IHttpClientFactory
                HttpClient cliente = new HttpClient();

                HttpResponseMessage response = await cliente.GetAsync(proveedorML.Uri, tokenCancelacion);

                respuestaML.httpStatusCode = response.StatusCode;
                respuestaML.JsonRespuesta = await response.Content.ReadAsStringAsync(tokenCancelacion);
            }
            catch (Exception ex)
            {
                respuestaML.Id = proveedorML.Id;
                respuestaML.httpStatusCode = HttpStatusCode.GatewayTimeout;
                Console.WriteLine(ex);
            }
            return respuestaML;

        }


    }
}
