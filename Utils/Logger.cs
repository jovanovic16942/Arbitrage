using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    public class Logger
    {
        public static ILogger? Instance { get; private set; }

        private static void InitLogger() 
        {
            ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging((loggingBuilder) => loggingBuilder
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole()
                )
            .BuildServiceProvider();

            Instance = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Program>();
        }

        public static ILogger GetLogger()
        {
            if (Instance == null)
            {
               InitLogger();
            }

            return Instance!;
        }
    }
}
