﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using e_Shop.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace e_Shop.Productos
{
    public class AzureProductosRepository : IProductosRepository
    {
        private string ConnectionString;
        public AzureProductosRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public async Task<bool> InsertarProducto(ProductoModel p)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("productos");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            var productoNuevo = new ProductoModelEntity(p.Codigo);
            productoNuevo.Categoria = p.Categoria;
            productoNuevo.Nombre = p.Nombre;
            productoNuevo.Precio = p.Precio;
            productoNuevo.Descripcion = p.Descripcion;

            var insertOp = TableOperation.Insert(productoNuevo);

            var res = await table.ExecuteAsync(insertOp);

            return true;
        }

        public Task<ProductoModel> LeerProductoPorCodigo(string Codigo)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductoModel>> LeerProductos()
        {
            throw new NotImplementedException();
        }
        private class ProductoModelEntity : TableEntity
        {
            public ProductoModelEntity(string Codigo)
            {
                this.PartitionKey = "Productos";
                this.RowKey = Codigo;
            }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public decimal Precio { get; set; }
            public string Descripcion { get; set; }
        }
    }
}