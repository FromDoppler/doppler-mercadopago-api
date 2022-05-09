namespace Doppler.MercadoPagoApi.Models
{
    public class PaymentResponseDto
    {
        public long? Id { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
    }
}
