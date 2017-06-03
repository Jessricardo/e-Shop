using e_Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace e_Shop.Repository
{
    interface IProductosRepository
    {
        Task<bool> InsertarProducto(ProductoModel p);
        ProductoModel LeerProductoPorCodigo(string Codigo);
        List<ProductoModel> LeerProductos();
        Task<bool> borrarProducto(string idProducto);
        Task<string> ActualizarImagen(string codigo, string fileName, Stream inputStream);
    }
}
