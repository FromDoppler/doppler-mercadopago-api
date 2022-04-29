using Loggly;
using Loggly.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Doppler.HelloMicroservice.Logging
{
    public static class LogglySetup
    {
        private const string DefaultEndpointHostname = "logs-01.loggly.com";
        private const int DefaultEndpointPort = 443;

        public static IConfiguration ConfigureLoggly(
            this IConfiguration configuration,
            IHostEnvironment hostingEnvironment,
            string appSettingsSection = nameof(LogglyConfig))
        {
            var config = LogglyConfig.Instance;

            // Set default values
            config.Transport.EndpointPort = DefaultEndpointPort;

            // Bind values from configuration
            configuration.GetSection(appSettingsSection).Bind(config);

            // Configure convention values if not set in configuration
            config.ApplicationName ??= hostingEnvironment.ApplicationName;
            config.Transport.EndpointHostname ??= DefaultEndpointHostname;

            return configuration;
        }
    }
}
