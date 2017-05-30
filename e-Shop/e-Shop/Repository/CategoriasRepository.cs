using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_Shop.Repository
{
    public class CategoriasRepository
    {
        private static List<string> categorias;
        static CategoriasRepository()
        {
            categorias = new List<string>();
            categorias.Add("Xbox");
            categorias.Add("Xbox One");
            categorias.Add("Wii U");
            categorias.Add("PlayStation 4");
        }
        public void AgregarCategoria(string categoria)
        {
            if (!categorias.Contains(categoria))
            {
                categorias.Add(categoria);
            }
        }
        public void EliminarCategoria(string categoria)
        {
            categorias.Remove(categoria);
        }
        public List<string> Todaslascategorias()
        {
            return categorias;
        }
    }
}