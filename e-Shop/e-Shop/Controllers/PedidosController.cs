using e_Shop.Models;
using e_Shop.Repository;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace e_Shop.Controllers
{
    public class PedidosController : Controller
    {
        private AzurePedidosRepository pedidos;
        private AzureCarritoRepository partidas;
        public string idUsuario = "null";
        public PedidosController()
        {
            partidas = new AzureCarritoRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            pedidos = new AzurePedidosRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
        // GET: Pedidos
        [Authorize]
        public async Task<ActionResult> Index()
        {
            bool val1 = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (val1)
            {
                idUsuario = System.Web.HttpContext.Current.User.Identity.Name;
            }
            var lista = await pedidos.leerPedidos(idUsuario);
            return View(lista);
        }

        // GET: Pedidos/Details/5
        [Authorize]
        public async Task<ActionResult> Details(string id)
        {
            var lista = await partidas.LeerPartidasPorPedido(id);
            return View(lista);
        }

        // GET: Pedidos/Create
        [Authorize]
        public async Task<ActionResult> Crear(double totalCant)
        {
            bool val1 = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (val1)
            {
                idUsuario = System.Web.HttpContext.Current.User.Identity.Name;
            }
            PedidoModel model =await pedidos.crearPedido(idUsuario,totalCant);
            return View(model);
        }

        // POST: Pedidos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pedidos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pedidos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pedidos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Pedidos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
