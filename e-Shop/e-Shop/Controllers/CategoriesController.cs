using e_Shop.Models;
using e_Shop.Repository;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_Shop.Controllers
{
    public class CategoriesController : Controller
    {
        private CategoriasRepository categorias;
        private IProductosRepository productos;
        public CategoriesController()
        {
            productos = new AzureProductosRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            categorias = new CategoriasRepository();
        }

        // GET: Categories
        public ActionResult Index()
        {
            return View(categorias.Todaslascategorias());
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: Categories/Details/categoria
        public ActionResult Details(string id)
        {
            List<ProductoModel> listaProductos = productos.LeerProductos();
            List<ProductoModel> nuevaLista = new List<ProductoModel>();
            foreach(ProductoModel prod in listaProductos)
            {
                if (prod.Categoria == id)
                {
                    nuevaLista.Add(prod);
                }
            }
            return View(nuevaLista);
        }

        // POST: Categories/Create
        [HttpPost]
        public ActionResult Create(string categoria)
        {
            try
            {
                categorias.AgregarCategoria(categoria);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // GET: Categories/Delete/5
        public ActionResult Delete(string categoria)
        {
            categorias.EliminarCategoria(categoria);
            return RedirectToAction("Index");
        }

        // POST: Categories/Delete/5
        [HttpPost]
        public ActionResult Delete(string categoria, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                categorias.EliminarCategoria(categoria);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
