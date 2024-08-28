using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace DolarApiConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DolarController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DolarController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("DolarOficial")]
        public async Task<IActionResult> GetDolarOficial()
        {
            var client = _httpClientFactory.CreateClient("DolarApi");
            var response = await client.GetAsync("v1/dolares/oficial");
            
            if (response.IsSuccessStatusCode)
            {
                var dolarOficial = await response.Content.ReadAsStringAsync();
                return Ok(dolarOficial);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error fetching dolar value");
            }
        }

        [HttpGet]
        [Route("DolarBlue")]
        public async Task<IActionResult> GetDolarBlue()
        {
            var client = _httpClientFactory.CreateClient("DolarApi");
            var response = await client.GetAsync("v1/dolares/blue");
            
            if (response.IsSuccessStatusCode)
            {
                var dolarBlue = await response.Content.ReadAsStringAsync();
                return Ok(dolarBlue);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error fetching dolar value");
            }
        }

        
        [HttpGet]
        [Route("Euro")]
        public async Task<IActionResult> GetEuroValue()
        {
            var client = _httpClientFactory.CreateClient("DolarApi");
            var response = await client.GetAsync("v1/cotizaciones/eur");
            
            if (response.IsSuccessStatusCode)
            {
                var euroValue = await response.Content.ReadAsStringAsync();
                return Ok(euroValue);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error fetching euro value");
            }
        }

        
        [HttpGet]
        [Route("Real")]
        public async Task<IActionResult> GetRealValue()
        {
            var client = _httpClientFactory.CreateClient("DolarApi");
            var response = await client.GetAsync("v1/cotizaciones/brl");
            
            if (response.IsSuccessStatusCode)
            {
                var realValue = await response.Content.ReadAsStringAsync();
                return Ok(realValue);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error fetching real value");
            }
        }
    }
}
