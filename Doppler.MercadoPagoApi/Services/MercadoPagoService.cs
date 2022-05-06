using MercadoPago.Client;
using MercadoPago.Client.Customer;
using MercadoPago.Resource;
using MercadoPago.Resource.Customer;
using System.Collections.Generic;
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

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            var searchRequest = new SearchRequest
            {
                Filters = new Dictionary<string, object>
                {
                    ["email"] = email,
                },
            };
            ResultsResourcesPage<Customer> results = await _customerClient.SearchAsync(searchRequest);

            if (results.Paging.Total > 0)
                return results.Results[0];

            return new Customer();
        }
    }
}
