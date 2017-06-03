using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using e_Shop.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace e_Shop.Repository
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
            productoNuevo.Codigo = p.Codigo;
            productoNuevo.Categoria = p.Categoria;
            productoNuevo.Nombre = p.Nombre;
            productoNuevo.Precio = p.Precio;
            productoNuevo.Descripcion = p.Descripcion;

            var insertOp = TableOperation.Insert(productoNuevo);

            var res = await table.ExecuteAsync(insertOp);

            return true;
        }

        public ProductoModel LeerProductoPorCodigo(string Codigo)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("productos");
            TableOperation retrieveOperation = TableOperation.Retrieve<ProductoModelEntity>("Productos", Codigo);
            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                ProductoModelEntity entity = ((ProductoModelEntity)retrievedResult.Result);
                ProductoModel producto = new ProductoModel()
                {
                    Categoria = entity.Categoria,
                    Codigo = entity.Codigo,
                    Descripcion = entity.Descripcion,
                    Nombre = entity.Nombre,
                    Precio = entity.Precio
                };
                return producto;
            }
            else
            {
                return (new ProductoModel());
            }
        }

        public List<ProductoModel> LeerProductos()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("productos");
            TableQuery<ProductoModelEntity> query = new TableQuery<ProductoModelEntity>();
            List<ProductoModel> lista = new List<ProductoModel>();
            foreach (ProductoModelEntity entity in table.ExecuteQuery(query))
            {
                ProductoModel producto = new ProductoModel()
                {
                    Categoria = entity.Categoria,
                    Codigo = entity.Codigo,
                    Descripcion = entity.Descripcion,
                    Nombre = entity.Nombre,
                    Precio = entity.Precio
                };
                lista.Add(producto);
            }
            return lista;
        }
        public async Task<bool> borrarProducto(string idProducto)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("productos");
            // Create the table if it doesn't exist.
            var deleteOp = TableOperation.Retrieve<ProductoModelEntity>("Productos", idProducto);
            // Execute the operation.
            TableResult retrievedResult = table.Execute(deleteOp);
            // Assign the result to a CustomerEntity.
            ProductoModelEntity deleteEntity = (ProductoModelEntity)retrievedResult.Result;
            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                // Execute the operation.
                table.Execute(deleteOperation);
                return true;
            }
            else
            {
                return false;
            }
        }
        private class ProductoModelEntity : TableEntity
        {
            public ProductoModelEntity(string Codigo)
            {
                this.PartitionKey = "Productos";
                this.RowKey = Codigo;
            }
            public ProductoModelEntity() { }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public double Precio { get; set; }
            public string Descripcion { get; set; }
        }
    }
}