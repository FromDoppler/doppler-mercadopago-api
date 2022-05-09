using MercadoPago.Resource.Payment;

namespace Doppler.MercadoPagoApi.Models
{
    public class CardDto
    {
        public string CardNumber { get; set; }
        public PaymentCardholder Cardholder { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string SecurityCode { get; set; }
    }
}
