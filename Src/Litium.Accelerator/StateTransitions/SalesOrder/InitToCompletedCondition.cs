using Litium.Accelerator.StateTransitions.SalesOrder;
using Litium.Sales;
using Litium.StateTransitions;

namespace Litium.Accelerator.StateTransitions
{
    public class InitToCompletedCondition : ProcessingToCompletedCondition
    {
        public InitToCompletedCondition(OrderOverviewService orderOverviewService, StateTransitionsService stateTransitionsService) : base(orderOverviewService, stateTransitionsService)
        {
        }

        public override string FromState => OrderState.Init;

        public override string ToState => OrderState.Completed;
    }
}
