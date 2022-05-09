using Doppler.MercadoPagoApi.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Payment;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public interface IMercadoPagoService
    {
        Task<Payment> CreatePaymentAsync(PaymentCreateRequest paymentCreateRequest);
        Task<CardToken> CreateTokenAsync(CardDto card);
    }
}
