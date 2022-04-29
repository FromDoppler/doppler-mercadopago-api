using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Doppler.HelloMicroservice.Logging;
using Serilog;

namespace Doppler.HelloMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostContext, loggerConfiguration) =>
                {
                    loggerConfiguration.SetupSeriLog(hostContext.Configuration, hostContext.HostingEnvironment);
                })
                .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
                {
                    // It is if you want to override the configuration in your
                    // local environment, `*.Secret.*` files will not be
                    // included in git.
                    configurationBuilder.AddJsonFile("appsettings.Secret.json", true);

                    // It is to override configuration using Docker's services.
                    // Probably this will be the way of overriding the
                    // configuration in our Swarm stack.
                    configurationBuilder.AddJsonFile("/run/secrets/appsettings.Secret.json", true);

                    // It is to override configuration using a different file
                    // for each configuration entry. For example, you can create
                    // the file `/run/secrets/Logging__LogLevel__Default` with
                    // the content `Trace`. See:
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#key-per-file-configuration-provider
                    configurationBuilder.AddKeyPerFile("/run/secrets", true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                // Validate composition root declaration in all environments
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });
    }
}
