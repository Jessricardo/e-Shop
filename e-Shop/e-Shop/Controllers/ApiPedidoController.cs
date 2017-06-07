using e_Shop.Models;
using e_Shop.Repository;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace e_Shop.Controllers
{
    public class ApiPedidoController : ApiController
    {
        private AzurePedidosRepository pedidos;
        private AzureCarritoRepository partidas;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApiPedidoController(){
            partidas = new AzureCarritoRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            pedidos = new AzurePedidosRepository(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
        public ApiPedidoController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: api/ApiPedido
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ApiPedido/5
        public async Task<List<PedidoModel>> Get(string id)
        {
            if (id != null)
            {
                var result = await pedidos.leerPedidos(id);
                return result;
            }
            return null;
        }
        [HttpGet]
        [Route("api/pedido/{id}")]
        public async Task<List<PartidaModel>> Detallado(string id)
        {
            var lista = await partidas.LeerPartidasPorPedido(id);
            return lista;
        }
        [HttpPost]
        [Route("api/login")]
        public async Task<Usuario> Login([FromBody]Usuario value)
        {
            var result = await SignInManager.PasswordSignInAsync(value.nombre, value.contraseña, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindByNameAsync(value.nombre);
                    Usuario nuevo = new Usuario()
                    {
                        APIKey = user.APIKey,
                        contraseña = "okey",
                        nombre = "chido"
                    };
                    return nuevo;
                case SignInStatus.LockedOut:
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    return null;
            }
        }

        // POST: api/ApiPedido
        public async Task<PedidoModel> Post([FromBody]PedidoPost value)
        {
            var user = await UserManager.FindByNameAsync(value.idUsuario);
            if (user.APIKey == value.APIKey)
            {
                var result = await pedidos.crearPedido(value.idUsuario, value.total);
                return result;
            }
            return null;
        }

        // PUT: api/ApiPedido/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiPedido/5
        public void Delete(int id)
        {
        }
        public class Usuario
        {
            public string nombre { get; set; }
            public string contraseña { get; set; }
            public string APIKey { get; set; }
        }
        public class PedidoPost
        {
            public string idUsuario { get; set; }
            public double total { get; set; }
            public string APIKey { get; set; }
        }
    }
}
