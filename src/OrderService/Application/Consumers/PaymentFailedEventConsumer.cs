using Application.Services.Abstract;
using Domain.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace Application.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedEventConsumer> _logger;
    private readonly IOrderService _orderService;

    public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, IOrderService orderService)
    {
        this._logger = logger;
        this._orderService = orderService;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var @event = context.Message;

        var order = await _orderService.GetByOrderId(@event.OrderId);
        if (order.Data != null)
        {
            var updateRequest = new OrderUpdateRequestDto()
            {
                OrderId = @event.OrderId,
                OrderStatus = Domain.Enums.OrderStatus.Failed,
                Message = "Order has been failed!"
            };
            await _orderService.Update(updateRequest);

            _logger.LogError($"{updateRequest.Message} - Order Id : {@event.OrderId}");
        }
    }
}
