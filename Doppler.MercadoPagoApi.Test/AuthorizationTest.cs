using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Doppler.HelloMicroservice
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
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20961002 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjo0MDAwMDAwMDAwfQ.Xm7dzdoK2MMGm9UmoHhuMID68LbYJi1Rk1NWl2BliGuzvVwVnNP3bvx3cHvylHa_xNVTAZDLzzyoLZCZSffWUUBdcSHJbXPdX42JZiLCE7AlWTfM85n4M84-5xmWWVEMY8KGwIKLJoE3EHWrRuv0AkO3ysWasoYhg3XQGVwxcw-sglM1eoo7TAiCxivgclLtTRfJZQ0_n3KkWGsTBJmfpHY2fMX1Mzr3RuBIr5Spwoni0dFRMQB8ilMlNl6GMuLPPRH2kjtzyk68U02_HniHdaABlblkJLRWHk6IOczajkvKJntdWsX_mbXHgNW49oaIy1CxR4Zh0XdrGjcMegLKkQ";
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoyMDAwMDAwMDAwfQ.E3RHjKx9p0a-64RN2YPtlEMysGM45QBO9eATLBhtP4tUQNZnkraUr56hAWA-FuGmhiuMptnKNk_dU3VnbyL6SbHrMWUbquxWjyoqsd7stFs1K_nW6XIzsTjh8Bg6hB5hmsSV-M5_hPS24JwJaCdMQeWrh6cIEp2Sjft7I1V4HQrgzrkMh15sDFAw3i1_ZZasQsDYKyYbO9Jp7lx42ognPrz_KuvPzLjEXvBBNTFsVXUE-ur5adLNMvt-uXzcJ1rcwhjHWItUf5YvgRQbbBnd9f-LsJIhfkDgCJcvZmGDZrtlCKaU1UjHv5c3faZED-cjL59MbibofhPjv87MK8hhdg";
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoxMDAwMDAwMDAwfQ.JBmiZBgKVSUtB4_NhD1kiUhBTnH2ufGSzcoCwC3-Gtx0QDvkFjy2KbxIU9asscenSdzziTOZN6IfFx6KgZ3_a3YB7vdCgfSINQwrAK0_6Owa-BQuNAIsKk-pNoIhJ-OcckV-zrp5wWai3Ak5Qzg3aZ1NKZQKZt5ICZmsFZcWu_4pzS-xsGPcj5gSr3Iybt61iBnetrkrEbjtVZg-3xzKr0nmMMqe-qqeknozIFy2YWAObmTkrN4sZ3AB_jzqyFPXN-nMw3a0NxIdJyetbESAOcNnPLymBKZEZmX2psKuXwJxxekvgK9egkfv2EjKYF9atpH5XwC0Pd4EWvraLAL2eg";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public AuthorizationTest(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Theory]
        [InlineData("/hello/anonymous", HttpStatusCode.OK)]
        public async Task GET_helloAnonymous_should_not_require_token(string url, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("/hello/anonymous", TOKEN_EMPTY, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_EXPIRE_20961002, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_EXPIRE_20010908, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_BROKEN, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20961002, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/anonymous", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908, HttpStatusCode.OK)]
        public async Task GET_helloAnonymous_should_accept_any_token(string url, string token, HttpStatusCode expectedStatusCode)
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
        [InlineData("/hello/valid-token", HttpStatusCode.Unauthorized)]
        [InlineData("/hello/superuser", HttpStatusCode.Unauthorized)]
        [InlineData("/accounts/123/hello", HttpStatusCode.Unauthorized)]
        [InlineData("/accounts/test1@test.com/hello", HttpStatusCode.Unauthorized)]
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
        [InlineData("/hello/valid-token", TOKEN_EMPTY, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/valid-token", TOKEN_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/valid-token", TOKEN_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/hello/valid-token", TOKEN_BROKEN, HttpStatusCode.Unauthorized, "invalid_token", "")]
        [InlineData("/hello/valid-token", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/valid-token", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/hello/valid-token", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/valid-token", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/hello/superuser", TOKEN_EMPTY, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/superuser", TOKEN_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/superuser", TOKEN_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/hello/superuser", TOKEN_BROKEN, HttpStatusCode.Unauthorized, "invalid_token", "")]
        [InlineData("/hello/superuser", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/superuser", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/hello/superuser", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/hello/superuser", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/123/hello", TOKEN_EMPTY, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/123/hello", TOKEN_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/123/hello", TOKEN_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/123/hello", TOKEN_BROKEN, HttpStatusCode.Unauthorized, "invalid_token", "")]
        [InlineData("/accounts/123/hello", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/123/hello", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/123/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/123/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_EMPTY, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_BROKEN, HttpStatusCode.Unauthorized, "invalid_token", "")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_SUPERUSER_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_SUPERUSER_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20961002, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token has no expiration\"")]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908, HttpStatusCode.Unauthorized, "invalid_token", "error_description=\"The token expired at")]
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
        [InlineData("/hello/valid-token", TOKEN_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/valid-token", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/valid-token", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/hello/valid-token", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.OK)]
        public async Task GET_helloValidToken_should_accept_valid_token(string url, string token, HttpStatusCode expectedStatusCode)
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
        [InlineData("/hello/superuser", TOKEN_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/hello/superuser", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/hello/superuser", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        public async Task GET_helloSuperUser_should_require_a_valid_token_with_isSU_flag(string url, string token, HttpStatusCode expectedStatusCode)
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
        [InlineData("/hello/superuser", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        public async Task GET_helloSuperUser_should_accept_valid_token_with_isSU_flag(string url, string token, HttpStatusCode expectedStatusCode)
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
        [InlineData("/accounts/123/hello", TOKEN_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/123/hello", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/456/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_SUPERUSER_FALSE_EXPIRE_20330518, HttpStatusCode.Forbidden)]
        [InlineData("/accounts/test2@test.com/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.Forbidden)]
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
        [InlineData("/accounts/123/hello", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/accounts/123/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_SUPERUSER_EXPIRE_20330518, HttpStatusCode.OK)]
        [InlineData("/accounts/test1@test.com/hello", TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518, HttpStatusCode.OK)]
        public async Task GET_account_endpoint_should_accept_valid_token_with_isSU_flag_or_a_token_for_the_right_account(string url, string token, HttpStatusCode expectedStatusCode)
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
    }
}
