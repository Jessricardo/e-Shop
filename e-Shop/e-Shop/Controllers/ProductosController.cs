using e_Shop.Models;
using e_Shop.Productos;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace e_Shop.Controllers
{
    public class ProductosController : Controller
    {
        private IProductosRepository productos;
        public ProductosController()
        {
            productos = new AzureProductosRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
        // GET: Productos
        public async Task<ActionResult> Index()
        {
            var model = await productos.LeerProductos();
            return View(model);
        }
        public ActionResult Crear()
        {
            var model = new ProductoModel();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Crear(ProductoModel model)
        {
            await productos.InsertarProducto(model);

            return RedirectToAction("Index");
        }
    }
}