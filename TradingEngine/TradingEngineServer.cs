using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Instrument;
using TradingEngineServer.Logging;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

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
                // Create orderbook instance
                Security security = new Security();
                Book orderBook = new Book(security);


                // Create mock buy orders
                Order mockOrder = new Order(new OrderCore(1, "User1", 1), 65, 1000, true);
                Order mockOrder2 = new Order(new OrderCore(2, "User1", 1), 67, 1200, true);
                Order mockOrder3 = new Order(new OrderCore(3, "User1", 1), 69, 800, true);
                Order mockOrder4 = new Order(new OrderCore(4, "User1", 1), 88, 4000, true);
                Order mockOrder5 = new Order(new OrderCore(5, "User1", 1), 10, 1500, true);

                orderBook.AddOrder(mockOrder);
                orderBook.AddOrder(mockOrder2);
                orderBook.AddOrder(mockOrder3);
                orderBook.AddOrder(mockOrder4);
                orderBook.AddOrder(mockOrder5);


                List<OrderbookEntry> buyOrderEntries = orderBook.GetBidOrders();
                foreach (var buyOrderEntry in buyOrderEntries)
                {
                    string buyOrderText = buyOrderEntry.GetFormattedString();
                    _logger.Information(nameof(TradingEngineServer), buyOrderText);
                }

                break;
                
            }
            _logger.Information(nameof(TradingEngineServer), "Stopping Trading Engine Server");
            return Task.CompletedTask;
        }
    }
}