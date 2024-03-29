using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace Application.Consumers;
public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedEventConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        this._logger = logger;
        this._publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var balance = 2000m;
        var @event = context.Message;

        if (balance > @event.Payment.TotalPrice)
        {
            _logger.LogInformation($"{@event.Payment.TotalPrice} TL was withdrawn from credit card for user id = {@event.BuyerId}");

            var paymentSuccessEvent = new PaymentSuccessEvent
            {
                BuyerId = @event.BuyerId,
                OrderId = @event.OrderId
            };

            await _publishEndpoint.Publish(paymentSuccessEvent);
        }
        else
        {
            _logger.LogError($"{@event.Payment.TotalPrice} TL was not withdrawn from credit card for user id = {@event.BuyerId}");

            var paymentFailedEvent = new PaymentFailedEvent
            {
                BuyerId = @event.BuyerId,
                OrderId = @event.OrderId,
                OrderItems = @event.OrderItems,
                Message = "Not Enough Balance!"
            };

            await _publishEndpoint.Publish(paymentFailedEvent);
        }
    }
}