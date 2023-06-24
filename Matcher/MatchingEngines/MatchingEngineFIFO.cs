using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;
using TradingEngineServer.Logging;
using System.Diagnostics;

namespace TradingEngineServer.Matcher
{
    public class MatchingEngineFIFO : IMatcher
    {
        private readonly ITextLogger _logger;
        // FIFO queue to store all incoming orders (both buy and sell sides)
        private readonly Queue<Order> _incomingOrders;

        // FIFO queue to store all trades executed
        private readonly Queue<Trade> _tradesExecuted = new Queue<Trade>();

        // orderbook that contains remaining orders
        private readonly Book _orderbook;

        public MatchingEngineFIFO(Security instrument, Queue<Order> incomingOrders, ITextLogger logger)
        {
            _logger = logger;
            _incomingOrders = incomingOrders;
            _orderbook = new Book(instrument);
        }

        public List<Trade> GetTrades()
        {
            throw new NotImplementedException();
        }

        public void AddTrade(Trade trade)
        {
            _tradesExecuted.Enqueue(trade);
            _logger.Information(nameof(Matcher), "Trade Executed...   " + trade.GetFormattedString());
        }

        public void ConsumeFullOrder(Order order)
        {
            _orderbook.RemoveOrder(new CancelOrder(order));
        }

        // consumes the order quantity by volume
        public void ConsumePartialOrder(Order order, uint volume)
        {
            _orderbook.ConsumePartialOrder(order.OrderId, volume);
        }

        public void AddToOrderbook(long filledQuantity, Order order)
        {
            // update the order quantity (reduce by quantity already filled)
            order.DecreaseQuantity((uint)filledQuantity);
            // store the remaining volume in the orderbook
            _orderbook.AddOrder(order);
            _logger.Information(nameof(Matcher), "Added to Book...    " + order.GetFormattedString());
        }

        /*
         *  Method to match an incoming order to the order(s) stored in the orderbook.
         *  The method follows price-time priority using a FIFO queue.
         *
         *  For every incoming buy order, we iterate through every ask order in the orderbook in ascending order and
         *  and consume as much of the ask order possible (depending on the buy order quantity) and move on to the next
         *  ask order when the incoming buy order has not been fully filled. The mechanism is similar for incoming sell order.
         */
        public void Match(Order order)
        {
            // check side
            Limit bestAsk = _orderbook.GetBestAsk();
            Limit bestBid = _orderbook.GetBestBid();

            // edge case when orderbook does not have bestAsk or bestBid
            if ((order.IsBuySide && bestAsk == null) || (!order.IsBuySide && bestBid == null))
            {
                AddToOrderbook(0, order);
                return;
            }

            if (order.IsBuySide && order.Price >= bestAsk.Price)
            {
                // we want to match the incoming buy order
                // keep track of the order quantity we have filled so far
                // NOTE: order quantity is not modified in the loop, we create a new order when we add the remaining quantity to the orderbook
                long filledQuantity = 0;
                List<OrderbookEntry> askOrders = _orderbook.GetAskOrders();
                foreach (var askOrderEntry in askOrders)
                {
                    Order askOrder = askOrderEntry.CurrentOrder;

                    // we have completely filled the incoming order
                    if (filledQuantity == order.CurrentQuantity)
                    {
                        break;
                    }

                    // do not consume ask (because ask price should be lower than requested)
                    if (askOrder.Price > order.Price)
                    {
                        break;
                    }

                    // Case 1: Ask order is fully consumed
                    if (filledQuantity + askOrder.CurrentQuantity <= order.CurrentQuantity)
                    {
                        filledQuantity += askOrder.CurrentQuantity;
                        // quantity traded = askOrder.CurrentQuantity
                        Trade resultingTrade = new Trade(askOrder.Price, askOrder.CurrentQuantity);
                        AddTrade(resultingTrade);
                        ConsumeFullOrder(askOrder);
                    }
                    // Case 2: Ask order is not fully consumed
                    else
                    {
                        // volume = quantity traded
                        long volume = order.CurrentQuantity - filledQuantity;
                        filledQuantity += volume;
                        Trade resultingTrade = new Trade(askOrder.Price, volume);
                        AddTrade(resultingTrade);
                        ConsumePartialOrder(askOrder, (uint)volume);
                    }
                }

                // incoming order has not been fully filled
                // NOTE: when incoming order has been fully filled, we need not add it into the orderbook
                if (filledQuantity < order.CurrentQuantity)
                {
                    AddToOrderbook(filledQuantity, order);
                }
            }

            else if (!order.IsBuySide && order.Price <= bestBid.Price)
            {
                long filledQuantity = 0;
                List<OrderbookEntry> bidOrders = _orderbook.GetBidOrders();
                foreach (var bidOrderEntry in bidOrders)
                {
                    Order bidOrder = bidOrderEntry.CurrentOrder;

                    // we have completely filled the incoming order
                    if (filledQuantity == order.CurrentQuantity)
                    {
                        break;
                    }

                    // do not consume bid (because bid price should be higher than requested)
                    if (bidOrder.Price < order.Price)
                    {
                        break;
                    }

                    // Case 1: Bid order is fully consumed
                    if (filledQuantity + bidOrder.CurrentQuantity <= order.CurrentQuantity)
                    {
                        filledQuantity += bidOrder.CurrentQuantity;
                        // quantity traded = bidOrder.CurrentQuantity
                        Trade resultingTrade = new Trade(bidOrder.Price, bidOrder.CurrentQuantity);
                        AddTrade(resultingTrade);
                        ConsumeFullOrder(bidOrder);
                    }
                    // Case 2: Bid order is not fully consumed
                    else
                    {
                        // volume = quantity traded
                        long volume = order.CurrentQuantity - filledQuantity;
                        filledQuantity += volume;
                        Trade resultingTrade = new Trade(bidOrder.Price, volume);
                        AddTrade(resultingTrade);
                        ConsumePartialOrder(bidOrder, (uint)volume);
                    }
                }

                // incoming order has not been fully filled
                // NOTE: when incoming order has been fully filled, we need not add it into the orderbook
                if (filledQuantity < order.CurrentQuantity)
                {
                    AddToOrderbook(filledQuantity, order);
                }

            }

            else
            {
                // order did not cross the spread, place in order book
                AddToOrderbook(0, order);
            }

        }

        public void Run()
        {
            var watch = Stopwatch.StartNew();

            _logger.Information(nameof(Matcher), "Starting Matching...");
            while (_incomingOrders.Any())
            {
                Order order = _incomingOrders.Dequeue();
                _logger.Information(nameof(TradingEngineServer), "Incoming order...   " + order.GetFormattedString());
                Match(order);
            }

            watch.Stop();
            var elapsedTimeInMilliseconds = watch.ElapsedMilliseconds;
            _logger.Information(nameof(Matcher), "Matching Process Took " + elapsedTimeInMilliseconds + " ms");

        }
    }
}