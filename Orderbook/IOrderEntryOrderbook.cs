using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    // Read and Write interface //
    public interface IOrderEntryOrderbook : IReadOnlyOrderbook
    {
        void AddOrder(Order order);
        void ChangeOrder(ModifyOrder modifyOrder);
        void CancelOrder(CancelOrder cancelOrder);

    }
}