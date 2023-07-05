using Litium.Sales;
using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.Shipment
{
    public class InitToProcessingCondition : StateTransitionValidationRule<Sales.Shipment>
    {
        private readonly OrderService _orderService;
        private readonly StateTransitionsService _stateTransitionsService;

        public InitToProcessingCondition(OrderService orderService, StateTransitionsService stateTransitionsService)
        {
            _orderService = orderService;
            _stateTransitionsService = stateTransitionsService;
        }
        public override string FromState => ShipmentState.Init;

        public override string ToState => ShipmentState.Processing;

        public override ValidationResult Validate(Sales.Shipment entity)
        {
            var result = new ValidationResult();
            var order = _orderService.Get<Order>(entity.OrderSystemId);
            if (order is null)
            {
                result.AddError("Order", $"Order {entity.OrderSystemId} is not found.");
                return result;
            }

            if (order is Sales.SalesOrder)
            {
                var orderState = _stateTransitionsService.GetState<Sales.SalesOrder>(order.SystemId);
                if (orderState == OrderState.Init)
                {
                    result.AddError("Shipment", "Could not move shipment to Processing, the Order is in Init State.");
                }
            }
            else if (order is Sales.SalesReturnOrder)
            {
                var orderState = _stateTransitionsService.GetState<Sales.SalesReturnOrder>(order.SystemId);
                if (orderState == SalesReturnOrderState.Init)
                {
                    result.AddError("Shipment", "Could not move shipment to Processing, the Sales Return Order is in Init State.");
                }
            }

            return result;
        }
    }
}
