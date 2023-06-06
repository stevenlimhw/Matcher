namespace TradingEngineServer.Orders
{
    public class Limit
    {
        public Limit(long price)
        {
            Price = price;
        }
        public long Price { get; set; }
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