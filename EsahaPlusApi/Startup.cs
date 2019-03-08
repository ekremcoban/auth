using Core.Core.Dtos;
using EsahaPlusApi.Attributes;
using EsahaPlusApi.Extensions;
using EsahaPlusApi.Helpers;
using EsahaPlusApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EsahaPlusApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _Environment { get; }
        private ILogger<Startup> _Logger { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _Environment = environment;
            _Logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DI for application services
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IJsonService, JsonService>();
            services.AddScoped<IFileService, FileService>();

            var serviceProvider = services.BuildServiceProvider();
            var jsonService = serviceProvider.GetService<IJsonService>();

            /*
             * CORS adjustments.
             * */
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                if (_Environment.IsDevelopment())
                    builder.AllowAnyOrigin(); //.WithOrigins("http://localhost:3000/");
                else
                    builder.AllowAnyOrigin();  //.WithOrigins("http://sutasbitportal", "http://sutasiis:8062");

                builder.WithMethods("OPTIONS", "HEAD", "GET", "PUT", "POST", "DELETE"/**/)
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            services.AddDistributedMemoryCache();

            /*
             * DB configuration.
             * */
            //string duwPass = Encoding.UTF8.GetString(Convert.FromBase64String(Configuration.GetConnectionString("DuwPass")));
            //string duwConnection = string.Format(Configuration.GetConnectionString("DuwConnection"), duwPass);
            //string logoConnection = string.Format(Configuration.GetConnectionString("LogoConnection"), duwPass);
            //string noviFarmConnection = string.Format(Configuration.GetConnectionString("NoviFarmConnection"), duwPass);
            //services.AddDbContext<DataUpdateWizardContext>(options => options.UseSqlServer(duwConnection), ServiceLifetime.Transient);
            //services.AddDbContext<LogoContext>(options => options.UseSqlServer(logoConnection), ServiceLifetime.Transient);
            //services.AddDbContext<NoviFarmContext>(options => options.UseSqlServer(noviFarmConnection), ServiceLifetime.Transient);

            //services.AddTransient<IUnitOfWork, UnitOfWork>();

            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                       

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var signingKey = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(jwt =>
            {
                jwt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                jwt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                jwt.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            //context.NoResult();
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        if (_Environment.IsDevelopment())
                            context.Response.WriteAsync(jsonService.SerializeObject(new ErrorDto() { Message = context.AuthenticateFailure.Message })).Wait();
                        else
                        {
                            if (context.Response.Headers.ContainsKey("Token-Expired"))
                            {
                                string error = jsonService.SerializeObject(new ErrorDto() { Message = "Bağlantı süreniz doldu!" });
                                _Logger.LogError(error);
                                context.Response.WriteAsync(error).Wait();
                            }
                            else
                            {
                                string error = jsonService.SerializeObject(new ErrorDto() { Message = "Bağlantı anahtarınız bulunmuyor!" });
                                _Logger.LogError(error);
                                context.Response.WriteAsync(error).Wait();
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services
                .AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add(new NoCacheAttribute());
                    options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));

                    if (!_Environment.IsDevelopment())
                        options.Filters.Add(new AjaxOnlyAttribute());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseMiddleware<SerilogExtraPropertyMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
                app.UseWebApiExceptionHandler();
            }

            // global cors policy
            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
