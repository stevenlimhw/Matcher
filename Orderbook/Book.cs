using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    /*
     *  Orderbook class stores the states of the orderbook entries, and provides functions
     *  that allows operations on those states (e.g. adding, changing, removing).
     */
    public class Book : IRetrievalOrderbook
    {
        private readonly Security _instrument;

        // dictionary key is orderId
        private readonly Dictionary<long, OrderbookEntry> _orders = new Dictionary<long, OrderbookEntry>();
        private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);
        private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(BidLimitComparer.Comparer);

        public Book(Security instrument)
        {
            _instrument = instrument;
        }

        public int Count => _orders.Count;

        public void AddOrder(Order order)
        {
            var baseLimit = new Limit(order.Price);
            AddOrder(order, baseLimit, order.IsBuySide ? _bidLimits : _askLimits, _orders);
        }

        private static void AddOrder(Order order, Limit baseLimit, SortedSet<Limit> limitLevels, Dictionary<long, OrderbookEntry> internalOrderbook)
        {
            if (limitLevels.TryGetValue(baseLimit, out Limit limit))
            {
                OrderbookEntry orderbookEntry = new OrderbookEntry(order, baseLimit);
                // edge case when limit exists but it has no orders on it
                if (limit.Head == null)
                {
                    // orderbookEntry will now be the only entry in the limit
                    limit.Head = orderbookEntry;
                    limit.Tail = orderbookEntry;
                }
                else
                {
                    // put the newly added order in front of the list,
                    // and shift the the rest of the entries back
                    OrderbookEntry tailPointer = limit.Tail;
                    tailPointer.Next = orderbookEntry;
                    orderbookEntry.Previous = tailPointer;
                    limit.Tail = orderbookEntry;
                }
                internalOrderbook.Add(order.OrderId, orderbookEntry);
            }
            else
            {
                // when level has not yet existed, we create the limit level
                limitLevels.Add(baseLimit);
                OrderbookEntry orderbookEntry = new OrderbookEntry(order, baseLimit);

                // initially, orderbook only has one entry
                baseLimit.Head = orderbookEntry;
                baseLimit.Tail = orderbookEntry;

                internalOrderbook.Add(order.OrderId, orderbookEntry);
            }
        }

        // remove/cancel order, then add new order
        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            if (_orders.TryGetValue(modifyOrder.OrderId, out OrderbookEntry orderbookEntry))
            {
                RemoveOrder(modifyOrder.ToCancelOrder());
                AddOrder(modifyOrder.ToNewOrder(), orderbookEntry.ParentLimit, modifyOrder.IsBuySide ? _bidLimits : _askLimits, _orders);
            }
        }

        public void RemoveOrder(CancelOrder cancelOrder)
        {
            if (_orders.TryGetValue(cancelOrder.OrderId, out var orderbookEntry))
            {
                RemoveOrder(cancelOrder.OrderId, orderbookEntry, _orders);
            }
        }

        private static void RemoveOrder(long orderId, OrderbookEntry orderbookEntry, Dictionary<long, OrderbookEntry> internalOrderbook)
        {
            // Manage the pointers of each orderbook entry on the linked list.
            if (orderbookEntry.Previous != null && orderbookEntry.Next != null)
            {
                orderbookEntry.Next.Previous = orderbookEntry.Previous;
                orderbookEntry.Previous.Next = orderbookEntry.Next;
            }
            else if (orderbookEntry.Previous != null)
            {
                orderbookEntry.Previous.Next = null;
            }
            else if (orderbookEntry.Next != null)
            {
                orderbookEntry.Next.Previous = null;
            }

            // Deal with the orderbook entries within the limit level.
            if (orderbookEntry.ParentLimit.Head == orderbookEntry && orderbookEntry.ParentLimit.Tail == orderbookEntry)
            {
                // there is one order on this level, so we simply remove this order
                orderbookEntry.ParentLimit.Head = null;
                orderbookEntry.ParentLimit.Tail = null;
            }
            else if (orderbookEntry.ParentLimit.Head == orderbookEntry)
            {
                // there is more than one order on this level, but the orderbookEntry is the first order
                orderbookEntry.ParentLimit.Head = orderbookEntry.Next;
            }
            else if (orderbookEntry.ParentLimit.Tail == orderbookEntry)
            {
                // there is more than one order on this level, but the orderbookEntry is the last order
                orderbookEntry.ParentLimit.Tail = orderbookEntry.Previous;
            }

            internalOrderbook.Remove(orderId);
        }

        public bool ContainsOrder(long orderId)
        {
            return _orders.ContainsKey(orderId);
        }

        public List<OrderbookEntry> GetAskOrders()
        {
            List<OrderbookEntry> orderbookEntries = new List<OrderbookEntry>();
            // get all orderbook entries for each limit level for the asks
            // askLimit is the starting node for the linked list containing all the ask entries for that limit level
            foreach (var askLimit in _askLimits)
            {
                if (askLimit.IsEmpty)
                {
                    continue;
                }
                OrderbookEntry askLimitPointer = askLimit.Head;
                while (askLimitPointer != null)
                {
                    orderbookEntries.Add(askLimitPointer);
                    askLimitPointer = askLimitPointer.Next;
                }
            }
            return orderbookEntries;
        }

        public List<OrderbookEntry> GetBidOrders()
        {
            List<OrderbookEntry> orderbookEntries = new List<OrderbookEntry>();
            // get all orderbok entries for each limit level for the bids
            // bidLimit is the starting node for the linked list containing all the bid entries for that limit level
            foreach (var bidLimit in _bidLimits)
            {
                if (bidLimit.IsEmpty)
                {
                    continue;
                }
                OrderbookEntry bidLimitPointer = bidLimit.Head;
                while (bidLimitPointer != null)
                {
                    orderbookEntries.Add(bidLimitPointer);
                    bidLimitPointer = bidLimitPointer.Next;
                }
            }
            return orderbookEntries;
        }

        public OrderbookSpread GetSpread()
        {
            long? bestAsk = null, bestBid = null;

            // case when there is a best ask (i.e. lowest price)
            if (_askLimits.Any() && !_askLimits.Min.IsEmpty)
            {
                bestAsk = _askLimits.Min.Price;
            }

            // case when there is a best bid (i.e. highest price)
            if (_bidLimits.Any() && !_bidLimits.Max.IsEmpty)
            {
                bestBid = _bidLimits.Max.Price;
            }
            return new OrderbookSpread(bestBid, bestAsk);
        }

        
    }
}