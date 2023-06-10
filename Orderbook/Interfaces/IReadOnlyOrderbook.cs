namespace TradingEngineServer.Orderbook
{
    // Read only interface //
    public interface IReadOnlyOrderbook
    {
        bool ContainsOrder(long orderId);
        OrderbookSpread GetSpread();
        int Count { get; }
    }
}