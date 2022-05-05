using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using MercadoPago.Client;
using MercadoPago.Client.Customer;
using MercadoPago.Resource;
using MercadoPago.Resource.Customer;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Doppler.HelloMicroservice.Services
{
    public class MercadoPagoServiceTest
    {
        private Mock<IMercadoPagoClient> _mpClient;
        private IMercadoPagoService _mercadoPagoService;
        private Customer _validCustomer;

        public MercadoPagoServiceTest()
        {
            var config = new Mock<IConfiguration>();
            _mpClient = new Mock<IMercadoPagoClient>();
            _mercadoPagoService = new MercadoPagoService(config.Object, _mpClient.Object);
            _validCustomer = new Customer
            {
                Id = "1117450528-p36mYwJDRhsVcx",
                Email = "test@fromdoppler.com",
                FirstName = "Mp",
                LastName = "Testing"
            };
        }

        [Fact]
        public void Create_customer_async_creates_new_client_when_not_exist()
        {
            // Arrange
            var emptyList = new List<Customer>();
            var emptyResult = CreateCustomerResultsPageFromList(emptyList);

            _mpClient.Setup(mpc => mpc.SearchCustomerAsync(It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(emptyResult));

            // Act
            _ = _mercadoPagoService.CreateCustomerAsync(new CustomerDto());

            // Assert
            _mpClient.Verify(mpc => mpc.CreateCustomerClientAsync(It.IsAny<CustomerRequest>()), Times.Once());
        }

        [Fact]
        public void Create_customer_async_returns_saved_customer_when_exists()
        {
            // Arrange
            var resultList = new List<Customer>
            {
                _validCustomer
            };
            var result = CreateCustomerResultsPageFromList(resultList);

            _mpClient.Setup(mpc => mpc.SearchCustomerAsync(It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(result));

            // Act
            _ = _mercadoPagoService.CreateCustomerAsync(new CustomerDto());

            //Assert
            _mpClient.Verify(mpc => mpc.CreateCustomerClientAsync(It.IsAny<CustomerRequest>()), Times.Never);
            _mpClient.Verify(mpc => mpc.SearchCustomerAsync(It.IsAny<SearchRequest>()), Times.Once);
        }

        [Fact]
        public void Get_customer_by_email_async_calls_search_customer()
        {
            // Arrange
            var email = "test@fromdoppler.com";

            // Act
            _ = _mercadoPagoService.GetCustomerByEmailAsync(email);

            // Assert
            _mpClient.Verify(mpc => mpc.SearchCustomerAsync(It.IsAny<SearchRequest>()), Times.Once);
        }

        private ResultsResourcesPage<Customer> CreateCustomerResultsPageFromList(IList<Customer> list)
        {
            return new ResultsResourcesPage<Customer>
            {
                Results = list,
                Paging = new ResultsPaging
                {
                    Limit = 0,
                    Offset = 0,
                    Total = list.Count(),
                }
            };
        }
    }
}
