using Litium.Accelerator.Constants;
using Litium.Accelerator.StateTransitions.SalesOrder;
using Litium.Sales;
using Litium.Tagging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Litium.Accelerator.StateTransitions
{
    [RunAsSystem]
    public class InitToConfirmedConditionTest : IClassFixture<SalesOrderFixture>
    {
        private readonly InitToConfirmedCondition _subject;
        private readonly OrderOverviewService _orderOverviewService;
        private readonly TaggingService _taggingService;
        private readonly SalesOrderFixture _salesOrderFixture;

        public InitToConfirmedConditionTest(SalesOrderFixture salesOrderFixture)
        {
            _orderOverviewService = salesOrderFixture.ServiceProvider.GetRequiredService<OrderOverviewService>();
            _taggingService = salesOrderFixture.ServiceProvider.GetRequiredService<TaggingService>();
            _subject = new InitToConfirmedCondition(_orderOverviewService, _taggingService);

            _salesOrderFixture = salesOrderFixture;
        }

        [Fact]
        public void Validate_WhenHaveNoTransactionTypeAuthorize_ShouldFailed()
        {
            var salesOrder = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Init, TransactionResult.Success);
            var result = _subject.Validate(salesOrder);

            Assert.False(result.Succeeded);
            Assert.NotEqual(0, result.Errors.Count);
        }

        [Fact]
        public void Validate_WhenHaveTransactionResultNotSuccess_ShouldFailed()
        {
            var salesOrder = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Authorize, TransactionResult.Failed);
            var result = _subject.Validate(salesOrder);

            Assert.False(result.Succeeded);
            Assert.NotEqual(0, result.Errors.Count);
        }

        [Fact]
        public void Validate_WhenViolateValidation_ShouldSuccess()
        {
            var salesOrder = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Authorize, TransactionResult.Success);
            var result = _subject.Validate(salesOrder);

            Assert.True(result.Succeeded);
            Assert.Equal(0, result.Errors.Count);
        }

        [Fact]
        public void Validate_CreatingWithOrganizationConnectd_ShouldFailed()
        {
            var salesOrder = _salesOrderFixture.CreateSalesOrderForOrganization(TransactionType.Authorize, TransactionResult.Success);
            _taggingService.Add<Order>(salesOrder.SystemId, OrderTaggingConstants.AwaitOrderApproval);
            var result = _subject.Validate(salesOrder);

            Assert.False(result.Succeeded);
            Assert.NotEqual(0, result.Errors.Count);
        }

    }
}
