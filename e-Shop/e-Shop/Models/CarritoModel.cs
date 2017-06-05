using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_Shop.Models
{
    public class CarritoModel
    {
        public string id { get; set; }
        public string fecha { get; set; }
        public string estado { get; set; }
    }

    public class PartidaModel
    {
        public string id { get; set; }
        public string idCarrito { get; set; }
        public string productoId { get; set; }
        public int cantidad { get; set; }
        public double costo { get; set; }
        public string nombre { get; set; }
        public string pedidoId { get; set; }
    }
}