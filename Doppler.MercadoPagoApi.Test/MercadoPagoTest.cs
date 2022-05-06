using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using MercadoPago.Client.Customer;
using MercadoPago.Error;
using MercadoPago.Resource.Customer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Doppler.MercadoPagoApi
{
    public class MercadoPagoTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoyMDAwMDAwMDAwfQ.E3RHjKx9p0a-64RN2YPtlEMysGM45QBO9eATLBhtP4tUQNZnkraUr56hAWA-FuGmhiuMptnKNk_dU3VnbyL6SbHrMWUbquxWjyoqsd7stFs1K_nW6XIzsTjh8Bg6hB5hmsSV-M5_hPS24JwJaCdMQeWrh6cIEp2Sjft7I1V4HQrgzrkMh15sDFAw3i1_ZZasQsDYKyYbO9Jp7lx42ognPrz_KuvPzLjEXvBBNTFsVXUE-ur5adLNMvt-uXzcJ1rcwhjHWItUf5YvgRQbbBnd9f-LsJIhfkDgCJcvZmGDZrtlCKaU1UjHv5c3faZED-cjL59MbibofhPjv87MK8hhdg";

        private WebApplicationFactory<Startup> _factory;
        private Mock<IMercadoPagoService> _mercadoPagoService;
        private readonly string _testEmail;
        private readonly string _validId;
        private readonly string _customerEndpoint;
        private readonly HttpClient _client;

        public MercadoPagoTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;

            _mercadoPagoService = new Mock<IMercadoPagoService>();
            _testEmail = "test1@test.com";
            _validId = "1119277628-Ovsq6Ydl0FBDfe";
            _customerEndpoint = $"/accounts/{_testEmail}/customer";
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(_mercadoPagoService.Object);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions());
        }

        [Fact]
        public async Task Create_customer_async_should_return_customer_id_when_is_created()
        {
            // Arrange
            var validCustomerDto = GetValidCustomerDto();

            _mercadoPagoService.Setup(mps => mps.CreateCustomerAsync(It.IsAny<CustomerRequest>()))
                .ReturnsAsync(GetValidCustomer()) ;

            var request = new HttpRequestMessage(HttpMethod.Post, _customerEndpoint)
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } },
                Content = JsonContent.Create(validCustomerDto),
            };
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(content, new { CustomerId = ""});

            Assert.Equal(HttpStatusCode.OK,response.StatusCode);
            Assert.Matches(_validId, result.CustomerId);
        }

        [Fact]
        public async Task Create_customer_async_should_return_bad_request_when_customer_exist()
        {
            // Arrange
            var mpresponse = new MercadoPago.Http.MercadoPagoResponse(400,new Dictionary<string,string>(),"Client already exists");
            var mpException = new MercadoPagoApiException("Error response from SDK",mpresponse);

            _mercadoPagoService.Setup(mps => mps.CreateCustomerAsync(It.IsAny<CustomerRequest>()))
                .ThrowsAsync(mpException);

            var request = new HttpRequestMessage(HttpMethod.Post, _customerEndpoint)
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } },
                Content = JsonContent.Create(GetValidCustomerDto()),
            };
            // Act

            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_customer_async_should_return_customer_id_when_exists()
        {
            // Arrange
            _mercadoPagoService.Setup(mps => mps.GetCustomerByEmailAsync(_testEmail))
                .ReturnsAsync(GetValidCustomer());
            var request = new HttpRequestMessage(HttpMethod.Get, _customerEndpoint)
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } }
            };
            //Act
            var response = await _client.SendAsync(request);
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(content, new { CustomerId = "" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_validId, result.CustomerId);
        }

        [Fact]
        public async Task Get_customer_async_should_return_no_content_when_customer_not_exists()
        {
            // Arrange
            _mercadoPagoService.Setup(mps => mps.GetCustomerByEmailAsync(_testEmail))
                .ReturnsAsync(new Customer());
            var request = new HttpRequestMessage(HttpMethod.Get, _customerEndpoint)
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } }
            };
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private CustomerDto GetValidCustomerDto()
        {
            return new CustomerDto
            {
                FirstName = "John",
                LastName = "Doe",
            };
        }

        private Customer GetValidCustomer()
        {
            return new Customer
            {
                Id = _validId,
                Email = _testEmail,
                FirstName = "John",
                LastName = "Doe",
            };
        }
    }
}
