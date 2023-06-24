using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Instrument;
using TradingEngineServer.Logging;
using TradingEngineServer.Matcher;
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
            _logger.Information(nameof(TradingEngineServer), "Starting Engine");
            while (!stoppingToken.IsCancellationRequested)
            {
                // Create orderbook instance
                Security security = new Security();
                Book orderBook = new Book(security);

                // Create a parser instance and process the test data
                DataParser parser = new DataParser();
                string filePath = "../Data/mock_data_small.csv";
                List<List<string>> parsedData = parser.ParseCsv(filePath);

                // Obtain the incoming orders
                Queue<Order> incomingOrders = new Queue<Order>();
                int orderId = 1;
                const string username = "Test";
                const int securityId = 1; // fixed for now
                foreach (List<string> row in parsedData)
                {
                    bool isBuySide = row[0].Trim() == "Bid" || row[0].Trim() == "B";
                    double price = double.Parse(row[1].Trim());
                    long quantity = long.Parse(row[2].Trim());
                    Order order = new Order(new OrderCore(orderId, username, securityId), price, (uint)quantity, isBuySide);
                    incomingOrders.Enqueue(order);
                    orderId += 1;
                }

                // Run the matching algorithm
                MatchingEngineFIFO matcher = new MatchingEngineFIFO(security, incomingOrders, _logger);
                matcher.Run();
                break;
            }
            _logger.Information(nameof(TradingEngineServer), "Stopping Engine");
            return Task.CompletedTask;
        }
    }
}