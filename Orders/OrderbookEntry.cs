namespace TradingEngineServer.Orders
{
    public class OrderbookEntry
    {
        public OrderbookEntry(Order currentOrder, Limit parentLimit)
        {
            CreationTime = DateTime.UtcNow;
            CurrentOrder = currentOrder;
            ParentLimit = parentLimit;
        }

        public DateTime CreationTime { get; private set; }
        public Order CurrentOrder { get; private set; }
        public Limit ParentLimit { get; private set; }
        public OrderbookEntry Next { get; set; }
        public OrderbookEntry Previous { get; set; }

        public string GetFormattedString()
        {
            return $"[{CreationTime:yyyy-MM-dd HH-mm-ss.fffffff}] " + CurrentOrder.GetFormattedString();
        }

    }
}