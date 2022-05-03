using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Doppler.HelloMicroservice
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
