using e_Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_Shop.Productos
{
    interface IProductosRepository
    {
        Task<bool> InsertarProducto(ProductoModel p);
        Task<ProductoModel> LeerProductoPorCodigo(string Codigo);
        Task<List<ProductoModel>> LeerProductos();
    }
}
