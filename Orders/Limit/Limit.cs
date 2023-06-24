namespace TradingEngineServer.Orders
{
    /*
     *  Class representing a price level (or price limit).
     *  Entries with the same price are on the same limit.
     *  OrderbookEntry instances will form the nodes of a linked list, and the Limit
     *  class will have a pointer to the first entry in the queue and the last entry in the queue.
     */
    public class Limit
    {
        public Limit(double price)
        {
            Price = price;
        }
        public double Price { get; set; }
        public OrderbookEntry Head { get; set; }
        public OrderbookEntry Tail { get; set; }
        
        /*
         * Function that computes the number of orders (not the quantity)
         * that are on this limit level of the orderbook.
         */
        public uint GetLevelOrderCount()
        {
            uint orderCount = 0;
            OrderbookEntry headPointer = Head;
            while (headPointer != null)
            {
                if (headPointer.CurrentOrder.CurrentQuantity != 0)
                {
                    orderCount++;
                }
                headPointer = headPointer.Next;
            }
            return orderCount;
        }

        /*
         * Function that computes the total quantity of the orders
         * that are on this limit level of the orderbook.
         */
        public uint GetLevelOrderQuantity()
        {
            uint orderQuantity = 0;
            OrderbookEntry headPointer = Head;
            while (headPointer != null)
            {
                orderQuantity += headPointer.CurrentOrder.CurrentQuantity;
                headPointer = headPointer.Next;
            }
            return orderQuantity;
        }

        /*
         *  Returns an list of immutable orders.
         */
        public List<OrderRecord> GetLevelOrderRecords()
        {
            List<OrderRecord> orderRecords = new List<OrderRecord>();
            OrderbookEntry headPointer = Head;
            uint TheoreticalQueuePosition = 0;
            while (headPointer != null)
            {
                var currentOrder = headPointer.CurrentOrder;
                if (currentOrder.CurrentQuantity != 0)
                {
                    orderRecords.Add(new OrderRecord(currentOrder.OrderId, currentOrder.CurrentQuantity,
                        Price, currentOrder.IsBuySide, currentOrder.Username, currentOrder.SecurityId, TheoreticalQueuePosition));
                }
                TheoreticalQueuePosition++;
                headPointer = headPointer.Next;
            }
            return orderRecords;
        }

        public bool IsEmpty 
        {
            get
            {
                return Head == null && Tail == null;
            }
        }
        public Side Side 
        {
            get
            {
                if (IsEmpty)
                {
                    return Side.Unknown;
                }
                else
                {
                    return Head.CurrentOrder.IsBuySide ? Side.Bid : Side.Ask;
                }
            }
        }
    }
}