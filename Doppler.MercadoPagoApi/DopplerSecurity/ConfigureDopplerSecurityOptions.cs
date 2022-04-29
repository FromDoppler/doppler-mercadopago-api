using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Doppler.HelloMicroservice.DopplerSecurity
{
    public class ConfigureDopplerSecurityOptions : IConfigureOptions<DopplerSecurityOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly IFileProvider _fileProvider;

        public ConfigureDopplerSecurityOptions(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _fileProvider = webHostEnvironment.ContentRootFileProvider;
        }

        private static string ReadToEnd(IFileInfo fileInfo)
        {
            using var stream = fileInfo.CreateReadStream();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static RsaSecurityKey ParseXmlString(string xmlString)
        {
            using var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.FromXmlString(xmlString);
            var rsaParameters = rsaProvider.ExportParameters(false);
            return new RsaSecurityKey(RSA.Create(rsaParameters));
        }

        public void Configure(DopplerSecurityOptions options)
        {
            var path = _configuration.GetValue(
                DopplerSecurityDefaults.PUBLIC_KEYS_FOLDER_CONFIG_KEY,
                DopplerSecurityDefaults.PUBLIC_KEYS_FOLDER_DEFAULT_CONFIG_VALUE);

            var filenameRegex = new Regex(_configuration.GetValue(
                DopplerSecurityDefaults.PUBLIC_KEYS_FILENAME_CONFIG_KEY,
                DopplerSecurityDefaults.PUBLIC_KEYS_FILENAME_REGEX_DEFAULT_CONFIG_VALUE));

            var files = _fileProvider.GetDirectoryContents(path)
                .Where(x => !x.IsDirectory && filenameRegex.IsMatch(x.Name));

            var publicKeys = files
                .Select(ReadToEnd)
                .Select(ParseXmlString)
                .ToArray();

            options.SigningKeys = publicKeys;
        }
    }
}
