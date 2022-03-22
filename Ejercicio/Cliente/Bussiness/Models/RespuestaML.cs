using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Models
{
    public class RespuestaML
    {
        public string  JsonRespuesta { set; get; }
        public HttpStatusCode httpStatusCode { set; get; }

        public string Id { set; get; }


    }
}
