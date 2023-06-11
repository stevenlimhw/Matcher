using TradingEngineServer.Orders;

namespace TradingEngineServer.Rejects
{
    public class Reject : IOrderCore
    {
        public Reject(IOrderCore rejectedOrder, RejectionReason rejectionReason)
        {
            RejectionReason = rejectionReason;
            _orderCore = rejectedOrder;
        }

        public RejectionReason RejectionReason { get; private set; }

        private readonly IOrderCore _orderCore;

        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;
    }
}