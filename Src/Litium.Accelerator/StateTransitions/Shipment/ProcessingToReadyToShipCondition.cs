using Litium.Sales;
using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.Shipment
{
    public class ProcessingToReadyToShipCondition : StateTransitionValidationRule<Sales.Shipment>
    {
        public ProcessingToReadyToShipCondition()
        {
        }

        public override string FromState => ShipmentState.Processing;

        public override string ToState => ShipmentState.ReadyToShip;

        public override ValidationResult Validate(Sales.Shipment entity)
        {
            var result = new ValidationResult();

            //We should allow the shipment to move to ready to ship, without checking its payment status.
            //The reason is that we cannot execute the capture for the partial shipment in case the payment does not partial capture.   

            return result;
        }
    }
}
