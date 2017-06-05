using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_Shop.Models
{
    public class PedidoModel
    {
        public string id { get; set; }
        public string idUsuario { get; set; }
        public string estado { get; set; }
        public double total { get; set; }
        public string cuenta { get; set; }
    }
}