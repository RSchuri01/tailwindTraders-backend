using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tailwind.Traders.WebBff.Controllers
{
    [Authorize]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CouponsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CouponsController> _logger;
        private readonly AppSettings _settings;
        private const string VERSION_API = "v1";

        public CouponsController(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options,
            ILogger<CouponsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _settings = options.Value;
        }

        // GET: v1/coupons
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCoupons()
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var result = await client.GetStringAsync(API.Coupons.GetCoupons(_settings.CouponsApiUrl, VERSION_API));
            var coupons = JsonConvert.DeserializeObject<Coupons>(result);

            result = await client.GetStringAsync(API.Products.GetRecommendedProducts(_settings.ProductsApiUrl, VERSION_API));
            var recommendedProducts = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);

            var aggresponse = new
            {
                Coupons = coupons,
                RecommendedProducts = recommendedProducts
            };
            return Ok(aggresponse);
        }
    }
}
