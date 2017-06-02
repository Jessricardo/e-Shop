using e_Shop.Models;
using e_Shop.Repository;
using Microsoft.Azure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace e_Shop.Controllers
{
    public class CarritoController : Controller
    {
        private AzureCarritoRepository partidas;
        public CarritoController()
        {
            partidas = new AzureCarritoRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
        // GET: Carrito
        public async Task<ActionResult> Index()
        {
            string idUsuario;
            bool val1 = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (val1) {
                idUsuario = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                if (Session["tokenSession"] == null)
                {
                    Session["tokenSession"] = Guid.NewGuid().ToString();
                }
                idUsuario = Session["tokenSession"].ToString();
            }
            var listaPartidas = await partidas.LeerPartidas(idUsuario);
            return View(listaPartidas);
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Carrito/agregar")]
        public async Task<PartidaModel> agregar([FromBody]PartidaModel value)
        {
            if (await partidas.InsertarPartida(value))
            {
                value.id = "Bien";
                return value;
            }
            else
            {
                value.id = "Mal";
                return value;
            }
        }
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("Carrito/quitar")]
        public async Task<HttpStatusCode> quitar([FromBody]PartidaModel viejaPartida)
        {
            if (await partidas.QuitarPartida(viejaPartida))
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.NotImplemented;
            }
        }
       
    }
}
