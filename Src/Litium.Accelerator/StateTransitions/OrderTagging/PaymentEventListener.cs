using System;
using System.Threading;
using System.Threading.Tasks;
using Litium.Accelerator.Constants;
using Litium.Events;
using Litium.Runtime;
using Litium.Sales;
using Litium.Sales.Events;
using Litium.Tagging;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.StateTransitions.OrderTagging
{
    [Autostart]
    public class PaymentEventListener : IAsyncAutostart
    {
        private readonly ILogger<TagEventListener> _logger;
        private readonly TaggingService _taggingService;
        private readonly OrderOverviewService _orderOverviewService;
        private readonly EventBroker _eventBroker;

        public PaymentEventListener(
            ILogger<TagEventListener> logger,
            TaggingService taggingService,
            OrderOverviewService orderOverviewService,
            EventBroker eventBroker)
        {
            _logger = logger;
            _taggingService = taggingService;
            _orderOverviewService = orderOverviewService;
            _eventBroker = eventBroker;
        }

        ValueTask IAsyncAutostart.StartAsync(CancellationToken cancellationToken)
        {
            _eventBroker.Subscribe<PaymentAuthorizeDenied>(x => AddOrderTags(x.Item.PaymentSystemId, OrderTaggingConstants.Attention, OrderTaggingConstants.PaymentDenied));
            _eventBroker.Subscribe<PaymentAuthorizeFailed>(x => AddOrderTags(x.Item.PaymentSystemId, OrderTaggingConstants.Attention, OrderTaggingConstants.PaymentFailed));
            _eventBroker.Subscribe<PaymentCaptureDenied>(x => AddOrderTags(x.Item.PaymentSystemId, OrderTaggingConstants.Attention, OrderTaggingConstants.PaymentDenied));
            _eventBroker.Subscribe<PaymentCaptureFailed>(x => AddOrderTags(x.Item.PaymentSystemId, OrderTaggingConstants.Attention, OrderTaggingConstants.PaymentFailed));
            return ValueTask.CompletedTask;
        }

        private void AddOrderTags(Guid paymentSystemId, params string[] tags)
        {
            var orderOverView = _orderOverviewService.GetByPayment(paymentSystemId);
            if (orderOverView is null)
            {
                _logger.LogDebug("The order ({SystemId}) is not exist.", orderOverView.SalesOrder.SystemId);
                return;
            }

            var tagsOrder = _taggingService.GetAll<Order>(orderOverView.SalesOrder.SystemId);
            foreach (var tag in tags)
            {
                if (!tagsOrder.Contains(tag))
                {
                    _taggingService.Add<Order>(orderOverView.SalesOrder.SystemId, tag);
                }
            }
        }
    }
}
