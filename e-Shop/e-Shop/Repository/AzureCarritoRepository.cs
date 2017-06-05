using e_Shop.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace e_Shop.Repository
{
    public class AzureCarritoRepository : ICarritoRepository
    {
        private string ConnectionString;
        public AzureCarritoRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public async Task CrearCarro(string idCarrito)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("carritos");
            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
            var carritoNuevo = new CarritoModelEntity(idCarrito);
            carritoNuevo.estado = "abierto";
            carritoNuevo.fecha = DateTime.Today.ToString();
            carritoNuevo.id = idCarrito;
            var insertOp = TableOperation.Insert(carritoNuevo);
            await table.ExecuteAsync(insertOp);
        }
        public bool existsCarrito(string idUsuario)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("carritos");
            table.CreateIfNotExists();
            TableOperation retrieveOperation = TableOperation.Retrieve<CarritoModelEntity>("Carritos", idUsuario);
            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                CarritoModelEntity carritoEntity = ((CarritoModelEntity)retrievedResult.Result);
                CarritoModel carrito = new CarritoModel()
                {
                    estado = carritoEntity.estado,
                    fecha = carritoEntity.fecha,
                    id = carritoEntity.id
                };
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> InsertarPartida(PartidaModel partida)
        {
            //if (!existsCarrito(partida.idCarrito)){
            //    await CrearCarro(partida.idCarrito);
            //}
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("partidas");
            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
            var partidaNueva = new PartidaModelEntity(partida.id);
            partidaNueva.id = partida.id;
            partidaNueva.idCarrito = partida.idCarrito;
            partidaNueva.cantidad = partida.cantidad;
            partidaNueva.productoId = partida.productoId;
            partidaNueva.costo = partida.costo;
            partidaNueva.nombre = partida.nombre;
            partidaNueva.pedidoId = partida.pedidoId;
            var insertOp = TableOperation.InsertOrReplace(partidaNueva);
            var res = await table.ExecuteAsync(insertOp);
            if (res.HttpStatusCode == 204)
            {
                return true;
            }
            return false;
        }

        public async Task<List<PartidaModel>> LeerPartidas(string idCarrito)
        {
            if (!existsCarrito(idCarrito))
            {
                await CrearCarro(idCarrito);
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("partidas");
            TableQuery<PartidaModelEntity> query = new TableQuery<PartidaModelEntity>().Where(TableQuery.GenerateFilterCondition("idCarrito", QueryComparisons.Equal, idCarrito));
            List<PartidaModel> lista = new List<PartidaModel>();
            foreach (PartidaModelEntity entity in table.ExecuteQuery(query))
            {
                PartidaModel partida = new PartidaModel()
                {
                    cantidad = entity.cantidad,
                    id = entity.id,
                    idCarrito = entity.idCarrito,
                    productoId = entity.productoId,
                    costo = entity.costo,
                    nombre = entity.nombre
                };
                lista.Add(partida);
            }
            return lista;
        }
        public async Task<List<PartidaModel>> LeerPartidasPorPedido(string idPedido)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("partidas");
            TableQuery<PartidaModelEntity> query = new TableQuery<PartidaModelEntity>().Where(TableQuery.GenerateFilterCondition("pedidoId", QueryComparisons.Equal, idPedido));
            List<PartidaModel> lista = new List<PartidaModel>();
            foreach (PartidaModelEntity entity in table.ExecuteQuery(query))
            {
                PartidaModel partida = new PartidaModel()
                {
                    cantidad = entity.cantidad,
                    id = entity.id,
                    idCarrito = entity.idCarrito,
                    productoId = entity.productoId,
                    costo = entity.costo,
                    nombre = entity.nombre
                };
                lista.Add(partida);
            }
            return lista;
        }
        public async Task<bool> QuitarPartida(PartidaModel viejaPartida)
        {
            string idPartida = viejaPartida.id;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("partidas");
            // Create the table if it doesn't exist.
            var deleteOp = TableOperation.Retrieve<PartidaModelEntity>("Partidas", idPartida);
            // Execute the operation.
            TableResult retrievedResult = table.Execute(deleteOp);
            // Assign the result to a CustomerEntity.
            PartidaModelEntity deleteEntity = (PartidaModelEntity)retrievedResult.Result;
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

        private class CarritoModelEntity : TableEntity
        {
            public CarritoModelEntity(string Codigo)
            {
                this.PartitionKey = "Carritos";
                this.RowKey = Codigo;
            }
            public CarritoModelEntity() { }
            public string id { get; set; }
            public string fecha { get; set; }
            public string estado { get; set; }
        }
        private class PartidaModelEntity : TableEntity
        {
            public PartidaModelEntity(string Codigo)
            {
                this.PartitionKey = "Partidas";
                this.RowKey = Codigo;
            }
            public PartidaModelEntity() { }
            public string id { get; set; }
            public string idCarrito { get; set; }
            public string productoId { get; set; }
            public int cantidad { get; set; }
            public double costo { get; set; }
            public string nombre { get; set; }
            public string pedidoId { get; set; }
        }
    }
    interface ICarritoRepository
    {
        bool existsCarrito(string idUsuario);
        Task CrearCarro(string idCarrito);
        Task<bool> InsertarPartida(PartidaModel partida);
        Task<List<PartidaModel>> LeerPartidas(string idCarrito);
        Task<bool> QuitarPartida(PartidaModel viejaPartida);
    }
}