using TradingEngineServer.Orders;

namespace TradingEngineServer.Matcher
{
    public interface IMatcher
    {
        void Match(Order order);
        void Run();
        List<Trade> getTrades();
    }
}