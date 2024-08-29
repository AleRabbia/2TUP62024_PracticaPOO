using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AfipFacturacion.Services;

namespace AfipFacturacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AfipController : ControllerBase
    {
        private readonly AfipWsfeClient _afipClient;
    
        public AfipController(AfipWsfeClient afipClient)
        {
            _afipClient = afipClient;
        }
    
        [HttpGet("UltimoNumeroFactura")]
        public async Task<IActionResult> ObtenerUltimoNumeroFactura(int puntoVenta, int tipoComprobante)
        {
            var ultimoNumero = await _afipClient.ObtenerUltimoNumeroFactura(puntoVenta, tipoComprobante);
            return Ok(new { UltimoNumeroFactura = ultimoNumero });
        }
}


}
