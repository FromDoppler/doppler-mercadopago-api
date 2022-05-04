using MercadoPago.Resource.Payment;

namespace Doppler.MercadoPagoApi.Models
{
    public class Card
    {
        public string CardNumber { get; set; }
        public PaymentCardholder CardHolder { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string SecurityCode { get; set; }
    }
}
