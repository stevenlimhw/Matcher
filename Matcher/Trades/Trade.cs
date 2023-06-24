namespace TradingEngineServer.Matcher
{
    public class Trade
    {
        public Trade(double price, long quantity)
        {
            Price = price;
            Quantity = quantity;
        }
        public double Price { get; set; }
        public long Quantity { get; set; }

        public String GetFormattedString()
        {
            return $"[Price: {Price}, Quantity: {Quantity}]";
        }
    }
}