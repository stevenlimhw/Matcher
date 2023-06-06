namespace TradingEngineServer.Orders
{
    // Read-only representation of an order.
    public record OrderRecord(long OrderId, uint Quantity, long Price, bool IsBuySide, 
        string Username, int SecurityId, uint TheoreticalQueuePosition);
}