using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.SalesOrder
{
    public class ConfirmedToProcessingCondition : StateTransitionValidationRule<Sales.SalesOrder>
    {
        public override string FromState => Sales.OrderState.Confirmed;

        public override string ToState => Sales.OrderState.Processing;

        public override ValidationResult Validate(Sales.SalesOrder entity)
        {
            //Empty condition and always returns no error.
            return new ValidationResult();
        }
    }
}
