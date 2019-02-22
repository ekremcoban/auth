using EsahaPlusApi.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EsahaPlusApi.Services
{
    public class JsonService : IJsonService
    {
        private IConfiguration _Configuration { get; }
        private IHostingEnvironment _Environment { get; }
        private AppSettings _AppSettings { get; }

        public JsonService(IOptions<AppSettings> appSettings, IHostingEnvironment environment)
        {
            _AppSettings = appSettings.Value;
            _Environment = environment;
        }
        
        public string SerializeObject(object obj)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            return JsonConvert.SerializeObject(obj, jsonSettings);
        }

        public string GetToken(string secret, IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //public Action<JwtBearerOptions> GetConfigurationOptions = (jwt) => {
        //    byte[] signingKey = Encoding.ASCII.GetBytes(_AppSettings.Secret);
        //    jwt.RequireHttpsMetadata = false;
        //    jwt.SaveToken = true;
        //    jwt.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(signingKey),
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ValidateLifetime = true,
        //        ClockSkew = TimeSpan.Zero
        //    };
        //    jwt.Events = new JwtBearerEvents()
        //    {
        //        OnAuthenticationFailed = context =>
        //        {
        //            if (context.Exception is SecurityTokenExpiredException)
        //            {
        //                //context.NoResult();
        //                context.Response.Headers.Add("Token-Expired", "true");
        //            }
        //            return Task.CompletedTask;
        //        },
        //        OnChallenge = context =>
        //        {
        //            context.HandleResponse();

        //            context.Response.StatusCode = 403;
        //            context.Response.ContentType = "application/json";

        //            if (_Environment.IsDevelopment())
        //                context.Response.WriteAsync(SerializeObject(new ErrorDto() { Message = context.AuthenticateFailure.Message })).Wait();
        //            else
        //            {
        //                /// TODO: Save log
        //                if (context.Response.Headers.ContainsKey("Token-Expired"))
        //                    context.Response.WriteAsync(SerializeObject(new ErrorDto() { Message = "Bağlantı süreniz doldu!" })).Wait();
        //                else
        //                    context.Response.WriteAsync(SerializeObject(new ErrorDto() { Message = "Bağlantı anahtarınız bulunmuyor!" })).Wait();
        //            }

        //            return Task.CompletedTask;
        //        }
        //    };
        //}
    }
}
