using Doppler.HelloMicroservice.DopplerSecurity;
using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IMercadoPagoService _mercadoPagoService;

        public PaymentController(IMercadoPagoService mercadoPagoService)
        {
            _mercadoPagoService = mercadoPagoService;
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpPost("/customers/{customername}")]
        public async Task<IActionResult> CreateCustomerAsync(CreateCustomerRequest request)
        {
            var customer = await _mercadoPagoService.CreateCustomerAsync(request.Customer);
            return Ok(new { CustomerId = customer.Id});
        }
    }
}
