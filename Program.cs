namespace SuumoScraping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Microsoft.Extensions.DependencyInjection;
    using SuumoScraping.Models;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (s, e) =>
                {
                    Console.WriteLine("Cancellation requested...");
                    cts.Cancel();
                    e.Cancel = true;
                };

                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    if (args[0] == "scrape")
                    {
                        var scraper = scope.ServiceProvider.GetRequiredService<SuumoScraper>();
                        Console.WriteLine("Starting scrape (Press Ctrl+C to stop)...");
                        scraper.Execute(cts.Token);
                        Console.WriteLine("Scrape finished.");
                        return;
                    }
                    else if (args[0] == "sync")
                    {
                        var service = scope.ServiceProvider.GetRequiredService<BukkenService>();
                        Console.WriteLine("Starting sync (Press Ctrl+C to stop)...");
                        service.Execute(cts.Token);
                        Console.WriteLine("Sync finished.");
                        return;
                    }
                }
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
