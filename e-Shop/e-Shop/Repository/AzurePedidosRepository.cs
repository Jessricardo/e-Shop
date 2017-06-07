using e_Shop.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace e_Shop.Repository
{
    public class AzurePedidosRepository : IPedidoRepository
    {
        private string ConnectionString;
        private AzureCarritoRepository partidas;
        public AzurePedidosRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
            partidas = new AzureCarritoRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

        public async Task<bool> actualizar(string id, string estado)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("pedidos");
            TableOperation retrieveOperation = TableOperation.Retrieve<PedidoModelEntity>("Pedidos", id);
            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                PedidoModelEntity entity = ((PedidoModelEntity)retrievedResult.Result);
                entity.estado = estado;
                table.CreateIfNotExists();
                var insertOp = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(insertOp);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<PedidoModel> crearPedido(string idUsuario,double total)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("pedidos");
            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
            string idPedido = Guid.NewGuid().ToString();
            var pedidoNuevo = new PedidoModelEntity(idPedido);
            pedidoNuevo.id = idPedido;
            pedidoNuevo.cuenta = Guid.NewGuid().ToString()+idUsuario;
            pedidoNuevo.estado = "No pagado";
            pedidoNuevo.idUsuario = idUsuario;
            pedidoNuevo.total = total;
            PedidoModel pedido = new PedidoModel()
            {
                cuenta = pedidoNuevo.cuenta,
                estado = pedidoNuevo.estado,
                id = pedidoNuevo.id,
                idUsuario = pedidoNuevo.idUsuario,
                total = pedidoNuevo.total
            }; 
            var insertOp = TableOperation.Insert(pedidoNuevo);
            await table.ExecuteAsync(insertOp);
            List<PartidaModel> listaPartidas = await partidas.LeerPartidas(idUsuario);
            foreach(PartidaModel objeto in listaPartidas)
            {
                PartidaModel actualizado = new PartidaModel()
                {
                    cantidad = objeto.cantidad,
                    costo = objeto.costo,
                    id = objeto.id,
                    idCarrito = "pedido",
                    nombre = objeto.nombre,
                    pedidoId = idPedido,
                    productoId = objeto.productoId
                };
                if (!await partidas.InsertarPartida(actualizado))
                {
                    break;                                      
                }
            }
            return pedido;
        }
        
        public async Task<List<PedidoModel>> leerPedidos(string idUsuario)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("pedidos");
            TableQuery<PedidoModelEntity> query = new TableQuery<PedidoModelEntity>().Where(TableQuery.GenerateFilterCondition("idUsuario", QueryComparisons.Equal, idUsuario));
            List<PedidoModel> lista = new List<PedidoModel>();
            foreach (PedidoModelEntity entity in table.ExecuteQuery(query))
            {
                PedidoModel pedido = new PedidoModel()
                {
                    idUsuario = entity.idUsuario,
                    cuenta = entity.cuenta,
                    estado = entity.estado,
                    id = entity.RowKey,
                    total =entity.total
                };
                lista.Add(pedido);
            }
            return lista;
        }

        public async Task<List<PedidoModel>> Todos()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("pedidos");
            TableQuery<PedidoModelEntity> query = new TableQuery<PedidoModelEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Pedidos"));
            List<PedidoModel> lista = new List<PedidoModel>();
            foreach (PedidoModelEntity entity in table.ExecuteQuery(query))
            {
                PedidoModel pedido = new PedidoModel()
                {
                    idUsuario = entity.idUsuario,
                    cuenta = entity.cuenta,
                    estado = entity.estado,
                    id = entity.RowKey,
                    total = entity.total
                };
                lista.Add(pedido);
            }
            return lista;
        }

        private class PedidoModelEntity :TableEntity
        {
            public PedidoModelEntity(string Codigo)
            {
                this.PartitionKey = "Pedidos";
                this.RowKey = Codigo;
            }
            public PedidoModelEntity() { }
            public string id { get; set; }
            public string idUsuario { get; set; }
            public string estado { get; set; }
            public double total { get; set; }
            public string cuenta { get; set; }
        }
    }
    interface IPedidoRepository
    {
        Task<PedidoModel> crearPedido(string idUsuario, double total);
        Task<List<PedidoModel>> leerPedidos(string idUsuario);
        Task<bool> actualizar(string id, string estado);
        Task<List<PedidoModel>> Todos();
    }
}