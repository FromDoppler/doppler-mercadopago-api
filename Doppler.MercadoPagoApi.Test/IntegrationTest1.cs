using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Doppler.MercadoPagoApi
{
    public class IntegrationTest1
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTest1(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
    }
}
