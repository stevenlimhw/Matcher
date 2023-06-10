namespace TradingEngineServer.Orderbook
{
    public interface IMatchingOrderbook : IRetrievalOrderbook
    {
        MatchResult Match();
    }
}