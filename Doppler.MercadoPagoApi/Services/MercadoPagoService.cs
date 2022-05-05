using Doppler.MercadoPagoApi.Models;
using MercadoPago.Client;
using MercadoPago.Client.Customer;
using MercadoPago.Config;
using MercadoPago.Resource;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Customer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Doppler.MercadoPagoApi.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly IMercadoPagoClient _mpClient;

        public MercadoPagoService(IConfiguration configuration, IMercadoPagoClient mpClient)
        {
            _configuration = configuration;
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
            _mpClient = mpClient;
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
            return await _mpClient.CreateCustomerAsync(customerRequest);
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
            ResultsResourcesPage<Customer> results = await _mpClient.SearchCustomerAsync(searchRequest);

            if (results.Paging.Total > 0)
                return results.Results[0];

            return new Customer();
        }
    }
}
