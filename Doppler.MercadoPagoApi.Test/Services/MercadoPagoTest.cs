using Doppler.MercadoPagoApi.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Error;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Common;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Doppler.MercadoPagoApi.Services
{
    public class MercadoPagoTest
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string TOKEN_SUPERUSER_VALID = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjp0cnVlLCJleHAiOjIwMDAwMDAwMDB9.rUtvRqMxrnQzVHDuAjgWa2GJAJwZ-wpaxqdjwP7gmVa7XJ1pEmvdTMBdirKL5BJIE7j2_hsMvEOKUKVjWUY-IE0e0u7c82TH0l_4zsIztRyHMKtt9QE9rBRQnJf8dcT5PnLiWkV_qEkpiIKQ-wcMZ1m7vQJ0auEPZyyFBKmU2caxkZZOZ8Kw_1dx-7lGUdOsUYad-1Rt-iuETGAFijQrWggcm3kV_KmVe8utznshv2bAdLJWydbsAUEfNof0kZK5Wu9A80DJd3CRiNk8mWjQxF_qPOrGCANOIYofhB13yuYi48_8zVPYku-llDQjF77BmQIIIMrCXs8IMT3Lksdxuw";
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly PaymentRequestDto _paymentRequestDto;
        private readonly Payment _payment;
        private readonly string _accountname;
        private readonly string _postUrl;
        private readonly string _getUrl;
        private readonly CardToken _cardToken;

        public MercadoPagoTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _accountname = "test1@example.com";
            _postUrl = $"/accounts/{_accountname}/payment";
            _getUrl = $"/accounts/{_accountname}/payment/1234567890";
            _cardToken = new CardToken { Id = "123asd" };

            #region paymentRequestDto sample
            _paymentRequestDto = new PaymentRequestDto
            {
                TransactionAmount = 9,
                Installments = 1,
                PaymentMethodId = "master",
                Card = new CardDto
                {
                    Cardholder = new PaymentCardholder
                    {
                        Identification = new Identification
                        {
                            Number = "12345678",
                            Type = "DNI"
                        },
                        Name = "APRO"
                    },
                    CardNumber = "5031755734530604",
                    SecurityCode = "123",
                    ExpirationMonth = "11",
                    ExpirationYear = "2025"
                }
            };
            #endregion
            #region payment Sample
            _payment = new Payment
            {
                Payer = new PaymentPayer()
            };
            #endregion
        }

        [Fact]
        public async Task POST_sendPayment_returns_OkStatusCode_when_request_success()
        {
            // Arrange
            var payment = new Payment
            {
                Id = 1,
                Status = "approved",
                StatusDetail = "accredited"
            };

            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.CreateTokenAsync(It.IsAny<CardDto>()))
                .ReturnsAsync(_cardToken);
            mercadoPagoServiceMock.Setup(s => s.CreatePaymentAsync(It.IsAny<PaymentCreateRequest>()))
                .ReturnsAsync(payment);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.PostAsJsonAsync(_postUrl, _paymentRequestDto);
            var result = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(payment.Id, result.Id);
        }

        [Fact]
        public async Task POST_sendPayment_returns_internalServerErrorStatusCode_when_unexpectedExceptions_occurs()
        {
            // Arrange
            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.CreateTokenAsync(It.IsAny<CardDto>()))
                .ReturnsAsync(_cardToken);
            mercadoPagoServiceMock.Setup(s => s.CreatePaymentAsync(It.IsAny<PaymentCreateRequest>()))
                .ThrowsAsync(new Exception());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.PostAsJsonAsync(_postUrl, _paymentRequestDto);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task POST_sendPayment_returns_internalServerErrorStatusCode_when_mercadoPagoExceptions_occurs()
        {
            // Arrange
            var mpResponse = new MercadoPago.Http.MercadoPagoResponse(400, new Dictionary<string, string>(), "");

            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.CreateTokenAsync(It.IsAny<CardDto>()))
                .ReturnsAsync(_cardToken);
            mercadoPagoServiceMock.Setup(s => s.CreatePaymentAsync(It.IsAny<PaymentCreateRequest>()))
                .ThrowsAsync(new MercadoPagoApiException("", mpResponse));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.PostAsJsonAsync(_postUrl, _paymentRequestDto);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GET_getPayment_returns_OkStatusCode_when_paymentId_exists()
        {
            // Arrange
            _payment.Payer.Email = _accountname;

            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.GetPaymentAsync(It.IsAny<long>()))
                .ReturnsAsync(_payment);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.GetAsync(_getUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_getPayment_returns_NotFoundStatusCode_when_paymentId_not_exists()
        {
            // Arrange
            var mpResponse = new MercadoPago.Http.MercadoPagoResponse(404, new Dictionary<string, string>(), "");

            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.GetPaymentAsync(It.IsAny<long>()))
                .ThrowsAsync(new MercadoPagoApiException("", mpResponse));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.GetAsync(_getUrl);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GET_getPayment_returns_internalServerErrorStatusCode_when_unexpectedExceptions_occurs()
        {
            // Arrange
            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.GetPaymentAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.GetAsync(_getUrl);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GET_getPayment_returns_UnauthorizedStatusCode_when_accountName_is_notEqual_to_payerEmail()
        {
            // Arrange
            _payment.Payer.Email = "different@example.com";

            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.GetPaymentAsync(It.IsAny<long>()))
                .ReturnsAsync(_payment);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN_SUPERUSER_VALID);

            // Act
            var response = await client.GetAsync(_getUrl);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
