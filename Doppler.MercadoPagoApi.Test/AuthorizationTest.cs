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
        
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public AuthorizationTest(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        
    }
}
