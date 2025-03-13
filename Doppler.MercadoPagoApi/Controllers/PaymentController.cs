using Doppler.MercadoPagoApi.DopplerSecurity;
using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using MercadoPago.Client.Payment;
using MercadoPago.Error;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public PaymentController(IMercadoPagoService mercadoPagoService, IConfiguration configuration, ILogger<PaymentController> logger)
        {
            _mercadoPagoService = mercadoPagoService;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpPost("/accounts/{accountname}/payment")]
        public async Task<IActionResult> Post([FromRoute] string accountname, [FromBody] PaymentRequestDto paymentRequestDto)
        {
            try
            {
                var cardToken = await _mercadoPagoService.CreateTokenAsync(paymentRequestDto.Card);
                var template = !paymentRequestDto.IsMontlhy ?
                    $"{_configuration["MercadoPago:NotificationEndpoint"]}?source_news=webhooks" :
                    $"{_configuration["MercadoPago:MonthlyNotificationEndpoint"]}?source_news=webhooks";
                var webhookNotificationsOnlyEndpoint = template.Replace("{accountname}", accountname);

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
                    Description = paymentRequestDto.Description,
                    NotificationUrl = webhookNotificationsOnlyEndpoint,
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
                _logger.LogError("Failed at creating MercadoPago payment for user {accountname} with exception message: {message}.", accountname, e.ApiError.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.ApiError);
            }
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpGet("/accounts/{accountname}/payment/{id}")]
        public async Task<IActionResult> Get([FromRoute] string accountname, [FromRoute] long id)
        {
            try
            {
                var result = await _mercadoPagoService.GetPaymentAsync(id);

                var resultWithEmail = result.Status is PaymentStatus.Approved or PaymentStatus.ChargedBack or PaymentStatus.Refunded;
                if (result.Payer.Email != accountname && resultWithEmail)
                    return Unauthorized();

                return Ok(result);
            }
            catch (MercadoPagoApiException e)
            {
                if (e.StatusCode == StatusCodes.Status404NotFound)
                    return NotFound(e.ApiError);

                _logger.LogError("Failed to get MercadoPago payment with id {id} for user {accountname} with exception message: {message}.", id, accountname, e.ApiError.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.ApiError);
            }
        }
    }
}
