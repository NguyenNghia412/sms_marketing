using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.lib.Stringee.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string? _apiSidKey;
        private readonly string? _apiSecretKey;
        private readonly string? _accountSidKey;
        private readonly string? _accountSecretKey;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiSidKey = _configuration["Stringee:ApiSIDKey"];
            _apiSecretKey = _configuration["Stringee:ApiSecretKey"];
            _accountSidKey = _configuration["Stringee:AccountSIDKey"];
            _accountSecretKey = _configuration["Stringee:AccountSecretKey"];


            if (string.IsNullOrEmpty(_apiSidKey) || string.IsNullOrEmpty(_apiSecretKey) || string.IsNullOrEmpty(_accountSidKey) || string.IsNullOrEmpty(_accountSecretKey))
            {
                throw new UserFriendlyException(ErrorCodes.System);
            }
        }

        public async Task<string> GenerateJwtTokenAsync(int expirationInMinutes = 60)
        {
            var now = DateTimeOffset.UtcNow;
            var expiration = now.AddMinutes(expirationInMinutes).ToUnixTimeSeconds();
            var timestamp = now.ToUnixTimeSeconds();
            var jti = $"{_apiSidKey}_{timestamp}";

            var header = new
            {
                typ = "JWT",
                alg = "HS256",
                cty = "stringee-api;v=1"
            };

            var payload = new
            {
                jti = jti,
                iss = _apiSidKey,
                exp = expiration,
                rest_api = true
            };

            var headerJson = JsonSerializer.Serialize(header);
            var payloadJson = JsonSerializer.Serialize(payload);

            var headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            var message = $"{headerEncoded}.{payloadEncoded}";
            var signature = CreateHmacSha256Signature(message, _apiSecretKey ?? "");
            var signatureEncoded = Base64UrlEncode(signature);

            var jwt = $"{headerEncoded}.{payloadEncoded}.{signatureEncoded}";

            return await Task.FromResult(jwt);
        }
        public async Task<string> GenerateAccountJwtTokenAsync(int expirationInMinutes = 60)
        {
            var now = DateTimeOffset.UtcNow;
            var expiration = now.AddMinutes(expirationInMinutes).ToUnixTimeSeconds();
            var timestamp = now.ToUnixTimeSeconds();
            var jti = $"{_accountSidKey}_{timestamp}";
            var header = new
            {
                typ = "JWT",
                alg = "HS256",
                cty = "stringee-api;v=1"
            };
            var payload = new
            {
                jti = jti,
                iss = _accountSidKey,
                exp = expiration,
                rest_api = true
            };
            var headerJson = JsonSerializer.Serialize(header);
            var payloadJson = JsonSerializer.Serialize(payload);
            var headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
            var message = $"{headerEncoded}.{payloadEncoded}";
            var signature = CreateHmacSha256Signature(message, _accountSecretKey ?? "");
            var signatureEncoded = Base64UrlEncode(signature);
            var jwt = $"{headerEncoded}.{payloadEncoded}.{signatureEncoded}";
            return await Task.FromResult(jwt);
        }

        private byte[] CreateHmacSha256Signature(string message, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            return hmac.ComputeHash(messageBytes);
        }

        private string Base64UrlEncode(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            return base64.Replace('+', '-')
                        .Replace('/', '_')
                        .Replace("=", "");
        }
    }
}