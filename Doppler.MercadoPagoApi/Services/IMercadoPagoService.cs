using MercadoPago.Client.Customer;
using MercadoPago.Resource.Customer;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public interface IMercadoPagoService
    {
        Task<Customer> CreateCustomerAsync(CustomerRequest request);
        Task<Customer> GetCustomerByEmailAsync(string email);
    }
}
