using e_Shop.Models;
using e_Shop.Repository;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace e_Shop.Controllers
{
    public class ListaProductosController : ApiController
    {
        private IProductosRepository productos;
        public ListaProductosController()
        {
            productos = new AzureProductosRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
        // GET: api/ListaProductos
        public IEnumerable<ProductoModel> Get()
        {
            return productos.LeerProductos();
        }

        // GET: api/ListaProductos/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ListaProductos
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ListaProductos/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ListaProductos/5
        public void Delete(int id)
        {
        }
    }
}
