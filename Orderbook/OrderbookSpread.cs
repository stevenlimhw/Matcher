using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public class OrderbookSpread
    {
        public OrderbookSpread(Limit? bid, Limit? ask)
        {
            Bid = bid;
            Ask = ask;
        }

        public Limit? Bid { get; private set; }
        public Limit? Ask { get; private set; }

        public long? Spread
        {
            get
            {
                if (Bid == null || Ask == null)
                {
                    return null;
                }
                return Ask.Price - Bid.Price;
            }
        }
    }
}