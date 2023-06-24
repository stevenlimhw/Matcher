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

                // orderBook.AddOrder(mockOrder);
                // orderBook.AddOrder(mockOrder2);
                // orderBook.AddOrder(mockOrder3);
                // orderBook.AddOrder(mockOrder4);
                // orderBook.AddOrder(mockOrder5);

                Queue<Order> incomingOrders = new Queue<Order>();
                incomingOrders.Enqueue(mockOrder);
                incomingOrders.Enqueue(mockOrder2);
                incomingOrders.Enqueue(mockOrder3);
                incomingOrders.Enqueue(mockOrder4);
                incomingOrders.Enqueue(mockOrder5);
                
                // _logger.Information(nameof(TradingEngineServer), incomingOrders.Peek().GetFormattedString());

                MatchingEngineFIFO matcher = new MatchingEngineFIFO(security, incomingOrders, _logger);
                matcher.Run();


                DataParser parser = new DataParser();
                string filePath = "../Data/mock_data.csv";
                List<List<string>> parsedData = parser.ParseCsv(filePath);

                foreach (List<string> row in parsedData)
                {
                    foreach (string field in row)
                    {
                        Console.Write(field + " ");
                    }
                    Console.WriteLine();
                }


                break;
            }
            _logger.Information(nameof(TradingEngineServer), "Stopping Trading Engine Server");
            return Task.CompletedTask;
        }
    }
}