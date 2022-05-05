using MercadoPago.Client;
using MercadoPago.Client.Customer;
using MercadoPago.Resource;
using MercadoPago.Resource.Customer;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public class MercadoPagoClient : IMercadoPagoClient
    {
        private readonly CustomerClient _customerClient;

        public MercadoPagoClient()
        {
            _customerClient = new CustomerClient();
        }

        public async Task<Customer> CreateCustomerAsync(CustomerRequest request)
        {
            return await _customerClient.CreateAsync(request);
        }

        public async Task<ResultsResourcesPage<Customer>> SearchCustomerAsync(SearchRequest request)
        {
            return await _customerClient.SearchAsync(request);
        }
    }
}
