using Doppler.MercadoPagoApi.Models;
using Doppler.MercadoPagoApi.Services;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.CardToken;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Doppler.MercadoPagoApi
{
    public class AuthorizationTest
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string TOKEN_EMPTY = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.e30.Nbd00AAiP2vJjxr78oPZoPRsDml5dx2bdD1Y6SXomfZN8pzJdKel2zrplvXCGBBYNBOo90rdYSlBCCo15rxsVydiFcAP84qZv-2mh4pFED9tVyDbxV5hvYDSg2bHPFyYFAi26fJusu_oYY3ne8OWxx-W1MEzNxh2hPfEKTkd0zVBm4dZv_irizRpa_qBwjn3hbCLUtOhBFbTTFItM9hESo6RwHvtQaB0667Sj8N97-bleCY5Ppf6bUUMz2A35PDb8-roF5Scf97lTZfug_DymgpPRSNK2VcRjfAynKfbBSih4QqVeaxR5AhYtXVFbQgByrynYNLok1SFD-M48WpzSA";
        const string TOKEN_BROKEN = "eyJhbGciOiJSzI1NiIsInR5cCI6IkpXVCJ9.e0.Nbd00AAiP2vJjxr8oPZoPRsDml5dx2bdD1Y6SXomfZN8pzJdKel2zrplvXCGBBYNBOo90rdYSlBCCo15rxsVydiFcAP84qZv-2mh4pFED9tVyDbxV5hvYDSg2bHPFyYFAi26fJusu_oYY3ne8OWxx-W1MEzNxh2hPfEKTkd0zVBm4dZv_irizRpa_qBwjn3hbCLUtOhBFbTTFItM9hESo6RwHvtQaB0667Sj8N97-bleCY5Ppf6bUUMz2A35PDb8-roF5Scf97lTZfug_DymgpPRSNK2VcRjfAynKfbBSih4QqVeaxR5AhYtXVbQgByrynYNLok1SFD-M48WpzSA";
        const string TOKEN_EXPIRE_20961002 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjQwMDAwMDAwMDB9.aOGfzmkPUKPE9dpIBvH1tMmCOAjLnNQ_nPulDc8dVW0eQbpII5ijDM_QHs8rRI4k7WQFml_AI-KigLqH2kloT58UaVU9UoYsJhPbM7cDYTMvs718EoopTJVCT5liPZM884m26YoFk9DE3GWkgh959kHZAWnzEFqDcaPUcrtcbbK4i9MPdJa_3Pu5tmWbWwdK0d3yIAuPWiQCAc-mbEqDwMCuI57gnX9RtnE1p-iflLxjjtjpovR0cSlwR6ESpQhhdBipFGjpvNOXxgS9ufxKGPg3e6UWN4SJUQzaskwh9QkZRFz_ca5Ge_yuGSQ_c6ZNJaNclkhxnH4BS5w7nnlUdQ";
        const string TOKEN_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjIwMDAwMDAwMDB9.mll33c0kstVIN9Moo4HSw0CwRjn0IuDc2h1wkRrv2ahQtIG1KV5KIxYw-H3oRfd-PiCWHhIVIYDP3mWDZbsOHTlnpRGpHp4f26LAu1Xp1hDJfOfxKYEGEE62Xt_0qp7jSGQjrx-vQey4l2mNcWkOWiE0plOws7cX-wLUvA3NLPoOvEegjM0Wx6JFcvYLdMGcTGT5tPd8Pq8pe9VYstCbhOClzI0bp81iON3f7VQP5d0n64eb_lvEPFu5OfURD4yZK2htyQK7agcNNkP1c5mLEfUi39C7Qtx96aAhOjir6Wfhzv_UEs2GQKXGTHl6_-HH-ecgOdIvvbqXGLeDmTkXUQ";
        const string TOKEN_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjEwMDAwMDAwMDB9.ZRjcfFTB973pD_gwB562BLCcszQOzubvr9TP6pWgA4wVIPeCzsX4waH7J9LPydY3pkp0UxaOffv-vJO0xZoWE9eUHdQbk8sy1CBgFM_dgyxY7DHKt0vuSjkPQ-VryPYwrTXO5lvaaDtMXIz6NdGC62oFQbvNOWD60790g2xzloge1bLpBYT1YRJK5dblA_mG9IJ1Id4R1HIZEmOIkOIhGU8-GQx2bP82xpudcEjOUZS7buRHpSy_Oy6fjy1KfUND_IbePuNF_t4n8Qo-MahshaphJrZlIKpEbw9gqlviH5s4lyU7AHhEs0JoTb2RGNTLq9h6m4Y-eMEFmPXnWN6dAA";
        const string TOKEN_SUPERUSER_EXPIRE_20961002 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjp0cnVlLCJleHAiOjQwMDAwMDAwMDB9.qLXdLXbGvZy_OkDGjJwkMoVBZhqEWLFR5oQtVxomauTg6gPAIGzKW8gZugFzrZnSG24chIY5_DhdlM93pnf8Tju803Q-CDbr4gI_2vsl-lxczqsf-Mk-wM09LeByQixuF8jMT5ICC1SNoZZ1-7ZkXe9WhF6hyowyXUy9ga73_ugfhrVOXgGImd6V9fAgR34Aiorqm3brzocZAB4MWDDNiO-Zf1CiDRDXnqwNareL2GtzGCC9H8FEDouSVovXWLzii13touavyEpIQ0XIbch09rTrpn00ZDHskEJtD8FI6zZPw26C48KfZFOlg4OwsFIl0v2UEEJs2uXHnVhL2_5nLQ";
        const string TOKEN_SUPERUSER_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjp0cnVlLCJleHAiOjIwMDAwMDAwMDB9.rUtvRqMxrnQzVHDuAjgWa2GJAJwZ-wpaxqdjwP7gmVa7XJ1pEmvdTMBdirKL5BJIE7j2_hsMvEOKUKVjWUY-IE0e0u7c82TH0l_4zsIztRyHMKtt9QE9rBRQnJf8dcT5PnLiWkV_qEkpiIKQ-wcMZ1m7vQJ0auEPZyyFBKmU2caxkZZOZ8Kw_1dx-7lGUdOsUYad-1Rt-iuETGAFijQrWggcm3kV_KmVe8utznshv2bAdLJWydbsAUEfNof0kZK5Wu9A80DJd3CRiNk8mWjQxF_qPOrGCANOIYofhB13yuYi48_8zVPYku-llDQjF77BmQIIIMrCXs8IMT3Lksdxuw";
        const string TOKEN_SUPERUSER_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjp0cnVlLCJleHAiOjEwMDAwMDAwMDB9.FYOpOxrXSHDif3lbQLPEStMllzEktWPKQ2T4vKUq5qgVjiH_ki0W0Ansvt0PMlaLHqq7OOL9XGFebtgUcyU6aXPO9cZuq6Od196TWDLMdnxZ-Ct0NxWxulyMbjTglUiI3V6g3htcM5EaurGvfu66kbNDuHO-WIQRYFfJtbm7EuOP7vYBZ26hf5Vk5KvGtCWha4zRM55i1-CKMhXvhPN_lypn6JLENzJGYHkBC9Cx2DwzaT683NWtXiVzeMJq3ohC6jvRpkezv89QRes2xUW4fRgvgRGQvaeQ4huNW_TwQKTTikH2Jg7iHbuRqqwYuPZiWuRkjqfd8_80EdlSAnO94Q";
        const string TOKEN_SUPERUSER_FALSE_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc1NVIjpmYWxzZSwiZXhwIjoyMDAwMDAwMDAwfQ.qMY3h8VhNxuOBciqrmXpTrRk8ElwDlT_3CYFzqJdXNjnJhKihFVMwjkWVw1EEckCWbKsRoBr-NgRV0SZ0JKWbMr2oGhZJWtqmKA05d8-i_MuuYbxtt--NUoQxg6AsMX989PGf6fSBzo_4szb7J0G6nUvvRxXfMnHMpaIAQUiBLNOoeKwnzsZFfI1ehmYGNmtc-2XyXOEHAnfZeBZw8uMWOp4A5hFBpVsaVCUiRirokjeCMWViVWT9NnVWbA60e_kfLjghEcXWaZfNnX9qtj4OC8QUB33ByUmwuYlTxNnu-qiEaJmbaaTeDD2JrKHf6MR59MlCHbb6BDWt20DBy73WQ";
        const string TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20961002 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUBleGFtcGxlLmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjo0MDAwMDAwMDAwfQ.wEkf8oi2H4ePTuRvAdDb9sA82wzwO-rQUP8O6CdIp6sp1eUDnfvljw35_n5NrghzVJzTy-C1Kqhb9OyTk5kbNouCFpfG9kWfxRIqEmSk1_X9Q6g9oMJtJ8VGvPcgnVZ_BA8d8t1wul1EaZrA0ydWuNxNaZ2vWvssHdkhHAWM9JeZ_MvHIvRR_QHYu_FEkwi0nA7xIJ3iW4Cjn4Jjhi_jLM4tnNlP0tBFDvM60qcfOf25hWZBOuSmAlpniQQPEN1_zE3owF486w2fJkQjNgvzsD7zRf6taXs_nQ-sWXNhxzY0R4bsgO1kUGXNSMG4uaaXbX_i4dOGuLKDD1sw-NXGDg";
        const string TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUBleGFtcGxlLmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoyMDAwMDAwMDAwfQ.C4shc2SZqolHSpxSLU3GykR0A0Zyh0fofqNirS3CmeY4ZerofgRry7m9AMFyn1SG-rmLDpFJIObFA2dn7nN6uKf5gCTEIwGAB71LfAeVaEfOeF1SvLJh3-qGXknqinsrX8tuBhoaHmpWpvdp0PW-8PmLuBq-D4GWBGyrP73sx_qQi322E2_PJGfudygbahdQ9v4SnBh7AOlaLKSXhGRT-qsMCxZJXpHM7cZsaBkOlo8x_LEWbbkf7Ub6q3mWaQsR30NlJVTaRMY9xWrRMV_iZocREg2EI33mMBa5zhuyQ-hXENp5M9FgS_9B-j3LpFJoJyVFZG2beBRxU8tnqKan3A";
        const string TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUBleGFtcGxlLmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoxMDAwMDAwMDAwfQ.Ite0xcvR2yLyFuVSBpoXeyJiwW44rYGJPGSX6VH_mCHImovvHMlcqJZkJLFy7A7jdUWJRZy23E_7tBR_rSEz9DBisiVksPeNqjuM3auUSZkPrRIYz16RZzLahhVNF-101j4Hg0Q7ZJB4zcT2a9qgB00CtSejUKrLoVljGj6mUz-ejVY7mNvUs0EE6e3pq4sylz9HHw0wZMBkv29xj_iE_3jBGwAwifh2UMQuBP_TAo6IiMaCMxmbPdITNEmQfXXIG3yPw6KwRjDw_EWR_R6yWFhbXuLONsZQF6b9mfokW9PxQ5MNCgvXihWCYaAibJ62R3N0pyUuvpjOJfifwFFaRA";


        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public AuthorizationTest(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment/1234567890", HttpStatusCode.Unauthorized)]
        public async Task GET_authenticated_endpoints_should_require_token(string url, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            // Act
            var response = await client.GetAsync(url);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_EMPTY, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_BROKEN, HttpStatusCode.Unauthorized, "invalid_token", "")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        public async Task GET_authenticated_endpoints_should_require_a_valid_token(string url, string token, HttpStatusCode expectedStatusCode, string error, string extraErrorInfo)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.StartsWith("Bearer", response.Headers.WwwAuthenticate.ToString());
            Assert.Contains($"error=\"{error}\"", response.Headers.WwwAuthenticate.ToString());
            Assert.Contains(extraErrorInfo, response.Headers.WwwAuthenticate.ToString());
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test2@example.com/payment/1234567890", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        public async Task GET_account_endpoint_should_require_a_valid_token_with_isSU_flag_or_a_token_for_the_right_account(string url, string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/accounts/test1@example.com/payment/1234567890", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20330518, HttpStatusCode.OK)]
        public async Task GET_account_endpoint_should_accept_valid_token_with_isSU_flag_or_a_token_for_the_right_account(string url, string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var payer = new PaymentPayer { Email = "test1@example.com" };

            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.GetPaymentAsync(It.IsAny<long>()))
                .ReturnsAsync(new Payment { Payer = payer });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment", HttpStatusCode.Unauthorized)]
        public async Task POST_authenticated_endpoints_should_require_token(string url, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_EMPTY, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_BROKEN, HttpStatusCode.Unauthorized, "invalid_token", "")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        public async Task POST_authenticated_endpoints_should_require_a_valid_token(string url, string token, HttpStatusCode expectedStatusCode, string error, string extraErrorInfo)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Headers = { { "Authorization", $"Bearer {token}" } },
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.StartsWith("Bearer", response.Headers.WwwAuthenticate.ToString());
            Assert.Contains($"error=\"{error}\"", response.Headers.WwwAuthenticate.ToString());
            Assert.Contains(extraErrorInfo, response.Headers.WwwAuthenticate.ToString());
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test2@example.com/payment", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        public async Task POST_account_endpoint_should_require_a_valid_token_with_isSU_flag_or_a_token_for_the_right_account(string url, string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/accounts/test1@example.com/payment", TOKEN_ACCOUNT_123_TEST1_AT_EXAMPLE_DOT_COM_EXPIRE_20330518, HttpStatusCode.OK)]
        public async Task POST_account_endpoint_should_accept_valid_token_with_isSU_flag_or_a_token_for_the_right_account(string url, string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            mercadoPagoServiceMock.Setup(s => s.CreateTokenAsync(It.IsAny<CardDto>()))
                .ReturnsAsync(new CardToken());
            mercadoPagoServiceMock.Setup(s => s.CreatePaymentAsync(It.IsAny<PaymentCreateRequest>()))
                .ReturnsAsync(new Payment());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mercadoPagoServiceMock.Object);
                });
            }).CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Headers = { { "Authorization", $"Bearer {token}" } },
                Content = JsonContent.Create(new PaymentRequestDto())
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}
