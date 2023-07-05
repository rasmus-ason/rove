using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.StateTransitions.SalesOrder;
using Litium.Accelerator.StateTransitions.Shipment;
using Litium.Events;
using Litium.Sales;
using Litium.Sales.Events;
using Litium.StateTransitions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Litium.Accelerator.StateTransitions
{
    [RunAsSystem]
    public class ProcessingToCompletedConditionTest : TestBase, IClassFixture<SalesOrderFixture>
    {
        private readonly ProcessingToCompletedCondition _subject;
        private readonly InitToProcessingCondition _subjectInitToProcessingCondition;
        private readonly OrderOverviewService _orderOverviewService;
        private readonly OrderService _orderService;
        private readonly SalesOrderFixture _salesOrderFixture;
        private readonly StateTransitionsService _stateTransitionsService;
        private readonly ShipmentService _shipmentService;
        private readonly TransactionService _transactionService;
        private readonly EventBroker _eventBroker;

        public ProcessingToCompletedConditionTest(SalesOrderFixture salesOrderFixture)
        {
            _orderOverviewService = salesOrderFixture.ServiceProvider.GetRequiredService<OrderOverviewService>();
            _orderService = salesOrderFixture.ServiceProvider.GetRequiredService<OrderService>();
            _stateTransitionsService = salesOrderFixture.ServiceProvider.GetRequiredService<StateTransitionsService>();
            _shipmentService = salesOrderFixture.ServiceProvider.GetRequiredService<ShipmentService>();
            _transactionService = salesOrderFixture.ServiceProvider.GetRequiredService<TransactionService>();
            _eventBroker = salesOrderFixture.ServiceProvider.GetRequiredService<EventBroker>();
            _subject = new ProcessingToCompletedCondition(_orderOverviewService, _stateTransitionsService);
            _subjectInitToProcessingCondition = new InitToProcessingCondition(_orderService, _stateTransitionsService);
            _salesOrderFixture = salesOrderFixture;
        }

        [Fact]
        public void Validate_WhenAllShipmentsAreNotShipped_ShouldFailed()
        {
            //Create order.
            var order = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Authorize, TransactionResult.Success);
            //Create shipment.
            var shipment = _salesOrderFixture.CreateShipment();
            shipment.OrderSystemId = order.SystemId;
            var splitOrderRow1 = new List<SalesOrderRow>() { order.Rows.First() };
            shipment.Rows = ConvertOrderRowsToShipmentRows(splitOrderRow1);
            _shipmentService.Create(shipment);
            AddDisposable(() => _shipmentService.Delete(shipment));
            var paymentSystemId = order.OrderPaymentLinks.First().PaymentSystemId;
            var transaction = _salesOrderFixture.CreateTransaction(paymentSystemId, TransactionType.Capture, TransactionResult.Success);
            transaction.Rows = ConvertShipmentRowsToTransactionRows(shipment.Rows);
            _transactionService.Create(transaction);
            AddDisposable(() => _transactionService.Delete(transaction));
            //Set Order state from Init to Confirm.
            Assert.True(_stateTransitionsService.SetState<Sales.SalesOrder>(shipment.OrderSystemId, OrderState.Confirmed).Success);

            _eventBroker.WaitForEvent<Shipped>(x => x.SystemId == shipment.SystemId, () =>
            {
                //Set shipment state from Init to Processing.
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Processing);
                //Set shipment state from Processing to ReadyToShip
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.ReadyToShip);
                //Set shipment state from ReadyToShip to Shipped
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Shipped);
            });

            var result = _subject.Validate(order);
            Assert.False(result.Succeeded);
            Assert.NotEqual(0, result.Errors.Count);
        }

        [Fact]
        public void Validate_WhenAllShipmentsAreNotShipped_ShouldSuccess()
        {
            // Create order.
            var order = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Authorize, TransactionResult.Success);
            var paymentSystemId = order.OrderPaymentLinks.First().PaymentSystemId;

            //Create shipment.
            var shipment = _salesOrderFixture.CreateShipment();
            shipment.OrderSystemId = order.SystemId;
            var splitOrderRow1 = new List<SalesOrderRow>() { order.Rows.First() };
            shipment.Rows = ConvertOrderRowsToShipmentRows(splitOrderRow1);
            _shipmentService.Create(shipment);
            AddDisposable(() => _shipmentService.Delete(shipment));
            var transaction = _salesOrderFixture.CreateTransaction(paymentSystemId, TransactionType.Capture, TransactionResult.Success);
            transaction.Rows = ConvertShipmentRowsToTransactionRows(shipment.Rows);
            _transactionService.Create(transaction);
            AddDisposable(() => _transactionService.Delete(transaction));
            //Set Order state from Init to Confirm.
            _stateTransitionsService.SetState<Sales.SalesOrder>(shipment.OrderSystemId, OrderState.Confirmed);

            _eventBroker.WaitForEvent<Shipped>(x => x.SystemId == shipment.SystemId, () =>
            {
                //Set shipment state from Init to Processing.
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Processing);
                //Set shipment state from Processing to ReadyToShip
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.ReadyToShip);
                //Set shipment state from ReadyToShip to Shipped
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Shipped);
            });

            var shipment2 = _salesOrderFixture.CreateShipment();
            shipment2.OrderSystemId = order.SystemId;
            var splitOrderRow2 = new List<SalesOrderRow>() { order.Rows.ToList()[1] };
            shipment2.Rows = ConvertOrderRowsToShipmentRows(splitOrderRow2);
            _shipmentService.Create(shipment2);
            AddDisposable(() => _shipmentService.Delete(shipment2));
            var transaction2 = _salesOrderFixture.CreateTransaction(paymentSystemId, TransactionType.Capture, TransactionResult.Success);
            transaction2.Rows = ConvertShipmentRowsToTransactionRows(shipment2.Rows);
            _transactionService.Create(transaction2);
            AddDisposable(() => _transactionService.Delete(transaction2));

            _eventBroker.WaitForEvent<Shipped>(x => x.SystemId == shipment2.SystemId, () =>
            {
                //Set shipment state from Init to Processing.
                _stateTransitionsService.SetState<Sales.Shipment>(shipment2.SystemId, ShipmentState.Processing);
                //Set shipment state from Processing to ReadyToShip
                _stateTransitionsService.SetState<Sales.Shipment>(shipment2.SystemId, ShipmentState.ReadyToShip);
                //Set shipment state from ReadyToShip to Shipped
                _stateTransitionsService.SetState<Sales.Shipment>(shipment2.SystemId, ShipmentState.Shipped);
            });

            var result = _subject.Validate(order);
            Assert.True(result.Succeeded);
            Assert.Equal(0, result.Errors.Count);
        }

        [Fact]
        public void StateTransition_WhenRelatedShipmentStateRun_ShouldOrderStateChange()
        {
            //Create an order.
            var order = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Authorize, TransactionResult.Success);
            var paymentSystemId = order.OrderPaymentLinks.First().PaymentSystemId;

            //Create a shipments - partial shipment.
            var orderRow2 = order.Rows.ToList()[1];
            var shipment = _salesOrderFixture.CreateShipment();
            shipment.OrderSystemId = order.SystemId;
            orderRow2.Quantity = 1;
            var splitOrderRow1 = new List<SalesOrderRow>() { orderRow2 };
            shipment.Rows = ConvertOrderRowsToShipmentRows(splitOrderRow1);
            _shipmentService.Create(shipment);
            AddDisposable(() => _shipmentService.Delete(shipment));

            var transaction = _salesOrderFixture.CreateTransaction(paymentSystemId, TransactionType.Capture, TransactionResult.Success);
            transaction.Rows = ConvertShipmentRowsToTransactionRows(shipment.Rows);
            _transactionService.Create(transaction);
            AddDisposable(() => _transactionService.Delete(transaction));

            var shipment2 = _salesOrderFixture.CreateShipment();
            shipment2.OrderSystemId = order.SystemId;
            var splitOrderRow2 = new List<SalesOrderRow>() { orderRow2, order.Rows.First() };
            shipment2.Rows = ConvertOrderRowsToShipmentRows(splitOrderRow2);
            _shipmentService.Create(shipment2);
            AddDisposable(() => _shipmentService.Delete(shipment2));

            var transaction2 = _salesOrderFixture.CreateTransaction(paymentSystemId, TransactionType.Capture, TransactionResult.Success);
            transaction2.Rows = ConvertShipmentRowsToTransactionRows(shipment2.Rows);
            _transactionService.Create(transaction2);
            AddDisposable(() => _transactionService.Delete(transaction2));

            _eventBroker.WaitForEvent<Shipped>(x => x.SystemId == shipment.SystemId, () =>
            {
                //Set order state from Init to Confirmed.
                _stateTransitionsService.SetState<Sales.SalesOrder>(order.SystemId, Sales.OrderState.Confirmed);
                //Set shipment state from Init to Processing.
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Processing);
                //Set shipment state from Processing to ReadyToShip
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.ReadyToShip);
                //Set shipment state from ReadyToShip to Shipped
                _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Shipped);
            });

            var shipmentState = _stateTransitionsService.GetState<Sales.Shipment>(shipment.SystemId);
            Assert.Equal(ShipmentState.Shipped, shipmentState);

            //Check related state.
            var orderState = _stateTransitionsService.GetState<Sales.SalesOrder>(order.SystemId);
            Assert.Equal(Sales.OrderState.Processing, orderState);

            _eventBroker.WaitForEvent<Shipped>(x => x.SystemId == shipment2.SystemId, () =>
            {
                //Set shipment state from Init to Processing.
                _stateTransitionsService.SetState<Sales.Shipment>(shipment2.SystemId, ShipmentState.Processing);
                //Set shipment state from Processing to ReadyToShip
                _stateTransitionsService.SetState<Sales.Shipment>(shipment2.SystemId, ShipmentState.ReadyToShip);
                //Set shipment state from ReadyToShip to Shipped
                _stateTransitionsService.SetState<Sales.Shipment>(shipment2.SystemId, ShipmentState.Shipped);
            });

            var shipment2State = _stateTransitionsService.GetState<Sales.Shipment>(shipment2.SystemId);
            Assert.Equal(ShipmentState.Shipped, shipment2State);

            //Check related state.
            var orderState1 = _stateTransitionsService.GetState<Sales.SalesOrder>(order.SystemId);
            Assert.Equal(Sales.OrderState.Completed, orderState1);
        }

        [Fact]
        public void Validate_WhenMoveShipmentToProcessing_WithOrderInitState_ShouldFailed()
        {
            // Create order.
            var order = _salesOrderFixture.CreateSalesOrderWithPayment(TransactionType.Authorize, TransactionResult.Success);

            //Create shipment.
            var shipment = _salesOrderFixture.CreateShipment();
            shipment.OrderSystemId = order.SystemId;
            var splitOrderRow1 = new List<SalesOrderRow>() { order.Rows.First() };
            shipment.Rows = ConvertOrderRowsToShipmentRows(splitOrderRow1);
            _shipmentService.Create(shipment);
            AddDisposable(() => _shipmentService.Delete(shipment));
            //Set Shipment state from Init to processing.
            _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Processing);
            var resultFailed = _subjectInitToProcessingCondition.Validate(shipment);
            Assert.False(resultFailed.Succeeded);

            _stateTransitionsService.SetState<Sales.SalesOrder>(order.SystemId, OrderState.Confirmed);
            //Set Shipment state from Init to processing.
            _stateTransitionsService.SetState<Sales.Shipment>(shipment.SystemId, ShipmentState.Processing);
            var resultSuccess = _subjectInitToProcessingCondition.Validate(shipment);
            Assert.True(resultSuccess.Succeeded);
        }

        private List<ShipmentRow> ConvertOrderRowsToShipmentRows(List<SalesOrderRow> orderRows)
        {
            var result = new List<ShipmentRow>();
            foreach (var item in orderRows.Where(x => x.OrderRowType == OrderRowType.Product))
            {
                result.Add(new ShipmentRow()
                {
                    OrderRowSystemId = item.SystemId,
                    ArticleNumber = item.ArticleNumber,
                    OrderRowType = item.OrderRowType,
                    ShippingInfoSystemId = item.ShippingInfoSystemId,
                    Id = this.UniqueString(),
                    Description = item.Description,
                    KeepAmountIncludingVatConstant = item.KeepAmountIncludingVatConstant,
                    ProductType = item.ProductType,
                    Quantity = item.Quantity,
                    TotalExcludingVat = item.TotalExcludingVat,
                    TotalIncludingVat = item.TotalIncludingVat,
                    TotalVat = item.TotalVat,
                    UnitOfMeasurementSystemId = item.UnitOfMeasurementSystemId,
                    UnitPriceExcludingVat = item.UnitPriceExcludingVat,
                    UnitPriceIncludingVat = item.UnitPriceIncludingVat,
                    VatRate = item.VatRate,
#pragma warning disable CS0618 // Type or member is obsolete
                    VatSummary = item.VatSummary,
#pragma warning restore CS0618 // Type or member is obsolete
                    VatDetails = item.VatDetails.Select(x => (VatDetail)((ICloneable)x).Clone()).ToList(),
                    SystemId = Guid.NewGuid()
                });
            }
            return result;
        }

        private static List<TransactionRow> ConvertShipmentRowsToTransactionRows(IEnumerable<ShipmentRow> shipmentRows)
        {
            var result = new List<TransactionRow>();
            foreach (var item in shipmentRows.Where(x => x.OrderRowType == OrderRowType.Product))
            {
                result.Add(new TransactionRow()
                {
                    OrderRowSystemId = item.OrderRowSystemId,
                    RowType = GetRowType(item.OrderRowType, item.ProductType),
                    ShipmentRowSystemId = item.SystemId,
                    Description = item.Description,
                    UnitPriceIncludingVat = item.UnitPriceIncludingVat,
                    UnitPriceExcludingVat = item.UnitPriceExcludingVat,
                    Quantity = item.Quantity,
                    VatRate = item.VatRate,
                    TotalIncludingVat = item.TotalIncludingVat,
                    TotalExcludingVat = item.TotalExcludingVat,
                    TotalVat = item.TotalVat,
                    AdditionalInfo = item.AdditionalInfo,
                    ArticleNumber = item.ArticleNumber,
#pragma warning disable CS0618 // Type or member is obsolete
                    VatSummary = item.VatSummary,
#pragma warning restore CS0618 // Type or member is obsolete
                    VatDetails = item.VatDetails.Select(x => (VatDetail)((ICloneable)x).Clone()).ToList(),
                    SystemId = Guid.NewGuid()
                });
            }
            return result;
        }

        private static TransactionRowType GetRowType(OrderRowType orderRowType, ProductType productType)
        {
            return orderRowType switch
            {
                OrderRowType.ShippingFee => TransactionRowType.ShippingFee,
                OrderRowType.Fee => TransactionRowType.Fee,
                OrderRowType.Discount => TransactionRowType.Discount,
                OrderRowType.RoundingOffAdjustment => TransactionRowType.RoundingOffAdjustment,
                _ => productType switch
                {
                    ProductType.DigitalGoods => TransactionRowType.DigitalGoods,
                    ProductType.PhysicalGoods => TransactionRowType.PhysicalGoods,
                    ProductType.Service => TransactionRowType.Service,
                    _ => TransactionRowType.Unknown,
                },
            };
        }
    }
}
