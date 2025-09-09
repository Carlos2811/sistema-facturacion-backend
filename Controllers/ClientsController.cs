using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using InvoiceApi.Services;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly InvoiceService _svc;
        public ClientsController(InvoiceService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var clients = await _svc.GetClientsAsync();
            return Ok(clients);
        }
    }
}