using TradingEngineServer.Logging;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Matcher
{
    public interface IMatcher
    {
        // matching logic for a single order
        void Match(Order order);

        // run the matching algorithm for every incoming order
        void Run();
        List<Trade> GetTrades();
        public void AddTrade(Trade trade);
    }
}