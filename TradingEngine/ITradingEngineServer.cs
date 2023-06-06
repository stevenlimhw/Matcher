namespace TradingEngineServer.Core
{
    public interface ITradingEngineServer
    {
        Task Run(CancellationToken token);
    }
}