using MercadoPago.Client.Customer;
using MercadoPago.Resource.Customer;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly CustomerClient _customerClient;

        public MercadoPagoService()
        {
            _customerClient = new CustomerClient();
        }

        public async Task<Customer> CreateCustomerAsync(CustomerRequest request)
        {
            return await _customerClient.CreateAsync(request);
        }
    }
}
