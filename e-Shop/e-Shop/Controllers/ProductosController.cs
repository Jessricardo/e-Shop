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
    public class ProductosController : Controller
    {
        private IProductosRepository productos;
        
        public ProductosController()
        {
            productos = new AzureProductosRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));            
        }
        // GET: Productos
        [Authorize]
        public ActionResult Index()
        {
            var model = productos.LeerProductos();
            return View(model);
        }
        [Authorize]
        public ActionResult Crear()
        {
            var model = new ProductoModel();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Crear(ProductoModel model,HttpPostedFileBase foto)
        {
            var urlFoto = await productos.ActualizarImagen(model.Codigo,foto.FileName, foto.InputStream);
            model.url = urlFoto;
            await productos.InsertarProducto(model);
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            return View();
        }
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

        public ActionResult Delete(string id)
        {
            return View(productos.LeerProductoPorCodigo(id));
        }

        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                productos.borrarProducto(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}