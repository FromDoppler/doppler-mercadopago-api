using Doppler.MercadoPagoApi.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Payment;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly PaymentClient _mercadoPagoPaymentClient;

        public MercadoPagoService(IConfiguration configuration, PaymentClient paymentClient)
        {
            _configuration = configuration;
            _mercadoPagoPaymentClient = paymentClient;
        }

        public async Task<Payment> CreatePaymentAsync(PaymentCreateRequest paymentCreateRequest)
        {
            var payment = await _mercadoPagoPaymentClient.CreateAsync(paymentCreateRequest);

            return payment;
        }

        public async Task<CardToken> CreateTokenAsync(CardDto card)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["MercadoPago:AccessToken"]}");
            var response = await client.PostAsJsonAsync(_configuration["MercadoPago:CreateTokenService"], card);
            var cardToken = await response.Content.ReadFromJsonAsync<CardToken>();

            return cardToken;
        }

        public async Task<Payment> GetPaymentAsync(long paymentId)
        {
            var payment = await _mercadoPagoPaymentClient.GetAsync(paymentId);

            return payment;
        }
    }
}
