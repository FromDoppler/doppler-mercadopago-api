namespace Doppler.MercadoPagoApi.Models
{
    public class CreateCustomerRequest
    {
        public Card Card { get; set; }
        public CustomerDto Customer { get; set; }
    }
}
