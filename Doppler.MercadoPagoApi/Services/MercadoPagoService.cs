using Doppler.MercadoPagoApi.Models;
using MercadoPago.Client;
using MercadoPago.Client.Customer;
using MercadoPago.Config;
using MercadoPago.Resource;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Customer;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly CustomerClient _customerClient;

        public MercadoPagoService(IConfiguration configuration)
        {
            _configuration = configuration;
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
            _customerClient = new CustomerClient();
        }

        public async Task<Customer> CreateCustomerAsync(CustomerDto customer)
        {
            var savedCustomer = await GetCustomerByEmailAsync(customer.Email);
            if (!string.IsNullOrEmpty(savedCustomer.Id))
                return savedCustomer;

            var customerRequest = new CustomerRequest
            {
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            };
            return await _customerClient.CreateAsync(customerRequest);
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

        public async Task<CardToken> CreateTokenAsync(Card card)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["MercadoPago:AccessToken"]}");
                var response = await client.PostAsJsonAsync(_configuration["MercadoPago:CreateTokenService"], card);
                return await response.Content.ReadFromJsonAsync<CardToken>();
            }
        }
    }
}
