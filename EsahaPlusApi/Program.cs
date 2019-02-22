using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace EsahaPlusApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();

            //string duwPass = Encoding.UTF8.GetString(Convert.FromBase64String(Configuration.GetConnectionString("EsahaPass")));
            //string duwConnection = string.Format(Configuration.GetConnectionString("EsahaConnection"), duwPass);

            /////
            ///// Configuration for Serilog.
            /////
            //string tableName = "Logs";

            //var columnOption = new ColumnOptions();
            //columnOption.Store.Remove(StandardColumn.MessageTemplate);

            //Log.Logger = new LoggerConfiguration()
            //                .Enrich.FromLogContext()
            //                //.WithProperty("Username", User.Identity.Name)
            //                .MinimumLevel.Error()
            //                .WriteTo.MSSqlServer(duwConnection, tableName, autoCreateSqlTable: true, columnOptions: columnOption)
            //                .CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            //    .UseSerilog()
            ;
    }
}
