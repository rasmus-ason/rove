using System;
using System.Linq;
using Litium.Sales;
using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.SalesOrder
{
    public class ProcessingToCompletedCondition : StateTransitionValidationRule<Sales.SalesOrder>
    {
        private readonly OrderOverviewService _orderOverviewService;
        private readonly StateTransitionsService _stateTransitionsService;

        public ProcessingToCompletedCondition(OrderOverviewService orderOverviewService, StateTransitionsService stateTransitionsService)
        {
            _orderOverviewService = orderOverviewService;
            _stateTransitionsService = stateTransitionsService;
        }

        public override string FromState => OrderState.Processing;

        public override string ToState => OrderState.Completed;

        public override ValidationResult Validate(Sales.SalesOrder entity)
        {
            var result = new ValidationResult();
            var order = _orderOverviewService.Get(entity.SystemId);

            //order can only be completed if shipments shipped.
            var hasAllShipmentShipped = HasAllShipmentsShipped(order);
            if(!hasAllShipmentShipped)
            {
                result.AddError("Shipment", "All shipments are not shipped.");
            }

            return result;
        }

        private bool HasAllShipmentsShipped(OrderOverview order)
        {
            var orderCountPerArticle = order.SalesOrder.Rows.Where(x => x.OrderRowType == OrderRowType.Product)
                                                            .GroupBy(r => r.ArticleNumber).ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity));
            var shipmentCountPerArticle = order.Shipments.Where(x => IsShippedOrCancelled(x.SystemId))
                                                         .SelectMany(x => x.Rows.Where(r => r.OrderRowType == OrderRowType.Product))
                                                         .GroupBy(r => r.ArticleNumber).ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity));

            return shipmentCountPerArticle.Sum(x => x.Value) == orderCountPerArticle.Sum(x => x.Value)
                   && orderCountPerArticle.All(x => shipmentCountPerArticle.TryGetValue(x.Key, out var value) && value == x.Value);


            bool IsShippedOrCancelled(Guid shipmentSystemId)
            {
                var shipmentState = _stateTransitionsService.GetState<Sales.Shipment>(shipmentSystemId);
                return shipmentState == ShipmentState.Shipped || shipmentState == ShipmentState.Cancelled;
            }
        }
    }
}
