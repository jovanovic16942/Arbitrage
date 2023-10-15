using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
