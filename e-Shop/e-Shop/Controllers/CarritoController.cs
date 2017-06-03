using e_Shop.Models;
using e_Shop.Repository;
using Microsoft.Azure;
using Newtonsoft.Json;
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
            var model = await partidas.LeerPartidas(idUsuario);
            return View(model);
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Carrito/detalles/{id}")]
        public async Task<string> detalles(string id)
        {
            List<PartidaModel> todos = await partidas.LeerPartidas(id);
            return JsonConvert.SerializeObject(todos);
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Carrito/agregar")]
        public async Task<HttpStatusCodeResult> agregar([FromBody]PartidaModel value)
        {
            if (await partidas.InsertarPartida(value))
            {
                return new HttpStatusCodeResult(200);
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Carrito/quitar")]
        public async Task<HttpStatusCodeResult> quitar([FromBody]PartidaModel viejaPartida)
        {
            if (await partidas.QuitarPartida(viejaPartida))
            {
                return new HttpStatusCodeResult(200);
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
        }
       
    }
}
