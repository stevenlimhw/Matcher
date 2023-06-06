using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    // Allows mutation of orderbook outside of the Orderbook class //
    public interface IRetrievalOrderbook : IOrderEntryOrderbook
    {
        List<OrderbookEntry> GetAskOrders();
        List<OrderbookEntry> GetBidOrders();
    }
}