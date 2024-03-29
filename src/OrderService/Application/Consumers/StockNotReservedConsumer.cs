using Application.Services.Abstract;
using Domain.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace Application.Consumers;
public class StockNotReservedConsumer : IConsumer<StockNotReservedEvent>
{
    private readonly ILogger<StockNotReservedConsumer> _logger;
    private readonly IOrderService _orderService;
    public StockNotReservedConsumer(ILogger<StockNotReservedConsumer> logger, IOrderService orderService)
    {
        this._logger = logger;
        this._orderService = orderService;
    }

    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        var @event = context.Message;

        var order = await _orderService.GetByOrderId(@event.OrderId);
        if (order.Data != null)
        {
            var updateRequest = new OrderUpdateRequestDto()
            {
                OrderId = @event.OrderId,
                OrderStatus = Domain.Enums.OrderStatus.Failed,
                Message = @event.Message
            };
            await _orderService.Update(updateRequest);

            _logger.LogError($"{updateRequest.Message} - Order Id : {@event.OrderId}");
        }
    }
}
