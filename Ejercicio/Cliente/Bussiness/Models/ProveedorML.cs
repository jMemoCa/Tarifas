using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Models
{
    public class ProveedorML
    {

        public string Id { get; set; }
        public string Uri { get; set; } 
         public int VecesSinResponder { set; get; }
        public bool Penalizado { get; set; }   
        public List<ReglaAsignacionML> ReglaAsignacionML { set; get; }

    }
}
