using TradingEngineServer.Orders;

namespace TradingEngineServer.Rejects
{
    public sealed class RejectCreator
    {
        public static Reject GenerateOrderCoreReject(IOrderCore rejectedOrder, RejectionReason rejectionReason)
        {
            return new Reject(rejectedOrder, rejectionReason);
        }
    }
}