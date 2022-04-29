using System;
using System.Linq;
using System.Net.Http;

namespace Doppler.HelloMicroservice
{
    public static class HttpResponseMessageExtensions
    {
        public static string GetHeadersAsString(this HttpResponseMessage response)
        {
            var keysAndValues = response.Headers.SelectMany(x => x.Value.Select(y => new { x.Key, Value = y }));
            var headerLines = keysAndValues.Select(x => $"{x.Key}: {x.Value}");
            return string.Join("\n", headerLines);
        }
    }
}
