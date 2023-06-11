using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Matcher
{
    public class MatchingEngineFIFO : IMatcher
    {
        // FIFO queue to store all incoming orders
        private readonly Queue<OrderbookEntry> _incomingOrders = new Queue<OrderbookEntry>();

        // FIFO queue to store all trades executed
        private readonly Queue<Trade> _tradesExecuted = new Queue<Trade>();

        // orderbook that contains remaining orders
        private readonly Book _orderbook;

        public MatchingEngineFIFO(Security instrument)
        {
            _orderbook = new Book(instrument);
        }

        public List<Trade> getTrades()
        {
            throw new NotImplementedException();
        }

        // matching logic for a single order
        public void Match(Order order)
        {
            // check side
            Limit bestAsk = _orderbook.GetBestAsk();
            if (order.IsBuySide && order.Price >= bestAsk.Price)
            {
                // we want to match the incoming buy order
                
            }
            
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}