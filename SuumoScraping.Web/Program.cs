namespace SuumoScraping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Application.UseCases;
    using SuumoScraping.Domain.Services;

    public class Program
    {
        public static async Task Main(string[] args)
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
                        await scraper.ExecuteAsync(cts.Token);
                        Console.WriteLine("Scrape finished.");
                        return;
                    }
                    else if (args[0] == "sync")
                    {
                        var useCase =
                            scope.ServiceProvider.GetRequiredService<SyncBukkensUseCase>();
                        Console.WriteLine("Starting sync (Press Ctrl+C to stop)...");
                        await useCase.ExecuteAsync(cts.Token);
                        Console.WriteLine("Sync finished.");
                        return;
                    }
                }
            }

            await CreateHostBuilder(args).Build().RunAsync();
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
