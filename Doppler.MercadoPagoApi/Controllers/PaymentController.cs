using Doppler.MercadoPagoApi.DopplerSecurity;
using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using MercadoPago.Client.Customer;
using MercadoPago.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        [HttpPost("/accounts/{accountname}/customer")]
        public async Task<IActionResult> CreateCustomerAsync([FromRoute] string accountname, CustomerDto customer)
        {
            var customerRequest = new CustomerRequest
            {
                Email = accountname,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            };
            try
            {
                var savedCustomer = await _mercadoPagoService.CreateCustomerAsync(customerRequest);
                return Ok(new { CustomerId = savedCustomer.Id });
            }
            catch (MercadoPagoApiException exception)
            {
                return BadRequest(exception.ApiError);
            }
        }
    }
}
