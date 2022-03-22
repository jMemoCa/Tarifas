using Bussiness.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Models;

namespace Bussiness.IServices
{
    public interface IHttpGet
    {
        public Task<RespuestaML> Get( ProveedorML proveedorML,CancellationToken tokenCancelacion);  
    }
}
