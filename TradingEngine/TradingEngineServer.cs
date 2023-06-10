using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;

namespace TradingEngineServer.Core
{
    public sealed class TradingEngineServer : BackgroundService, ITradingEngineServer
    {
        private readonly ITextLogger _logger;
        private readonly IOptions<TradingEngineServerConfiguration> _tradingEngineServerConfig;
        public TradingEngineServer(IOptions<TradingEngineServerConfiguration> config, ITextLogger textLogger)
        {
            _tradingEngineServerConfig = config ?? throw new ArgumentNullException(nameof(config));
            _logger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));
        }

        public Task Run(CancellationToken token) => ExecuteAsync(token);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information(nameof(TradingEngineServer), "Starting Trading Engine Server");
            while (!stoppingToken.IsCancellationRequested)
            {
                for (int i = 0; i < 10; i++)
                {
                    _logger.Information(nameof(TradingEngineServer), i.ToString());
                }
            }
            _logger.Information(nameof(TradingEngineServer), "Stopping Trading Engine Server");
            return Task.CompletedTask;
        }
    }
}