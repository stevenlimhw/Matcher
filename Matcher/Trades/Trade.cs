namespace TradingEngineServer.Matcher
{
    public class Trade
    {
        public Trade(long price, long quantity)
        {
            Price = price;
            Quantity = quantity;
        }
        public long Price { get; set; }
        public long Quantity { get; set; }

        public String GetFormattedString()
        {
            return $"[Price: {Price}, Quantity: {Quantity}]";
        }
    }
}