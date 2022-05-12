using Doppler.MercadoPagoApi.DopplerSecurity;
using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using MercadoPago.Client.Payment;
using MercadoPago.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMercadoPagoService _mercadoPagoService;

        public PaymentController(IMercadoPagoService mercadoPagoService)
        {
            _mercadoPagoService = mercadoPagoService;
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpPost("/accounts/{accountname}/payment")]
        public async Task<IActionResult> Post([FromRoute] string accountname, [FromBody] PaymentRequestDto paymentRequestDto)
        {
            try
            {
                var cardToken = await _mercadoPagoService.CreateTokenAsync(paymentRequestDto.Card);

                var paymentRequestCreated = new PaymentCreateRequest
                {
                    TransactionAmount = paymentRequestDto.TransactionAmount,
                    StatementDescriptor = paymentRequestDto.TransactionDescription,
                    Token = cardToken.Id,
                    Installments = paymentRequestDto.Installments,
                    PaymentMethodId = paymentRequestDto.PaymentMethodId,
                    Payer = new PaymentPayerRequest
                    {
                        Email = accountname,
                    },
                };

                var paymentResponse = await _mercadoPagoService.CreatePaymentAsync(paymentRequestCreated);

                var paymentResponseDto = new PaymentResponseDto
                {
                    Id = paymentResponse.Id,
                    Status = paymentResponse.Status,
                    StatusDetail = paymentResponse.StatusDetail,
                };

                return Ok(paymentResponseDto);
            }
            catch (MercadoPagoApiException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.ApiError);
            }
        }
    }
}
