using MercadoPago.Client;
using MercadoPago.Client.Customer;
using MercadoPago.Resource;
using MercadoPago.Resource.Customer;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public interface IMercadoPagoClient
    {
        Task<Customer> CreateCustomerClientAsync(CustomerRequest request);

        Task<ResultsResourcesPage<Customer>> SearchCustomerAsync(SearchRequest request);
    }
}
