namespace Doppler.MercadoPagoApi.Models
{
    public class PaymentRequestDto
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public CardDto Card { get; set; }
        public int Installments { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
