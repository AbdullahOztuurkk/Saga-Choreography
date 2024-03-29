using Application.Services.Abstract;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace Application.Consumers;
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IStockService _stockService;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;
    public OrderCreatedEventConsumer(IStockService stockService,
        ILogger<OrderCreatedEventConsumer> logger,
        ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEndpoint)
    {
        this._stockService = stockService;
        this._logger = logger;
        this._sendEndpointProvider = sendEndpointProvider;
        this._publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var @event = context.Message;
        var result = await _stockService.CheckStock(@event.OrderItems);
        if (result.IsSuccess)
        {
            for (int index = 0; index < @event.OrderItems.Count; index++)
            {
                var stock = (await _stockService.GetByProductId(@event.OrderItems[index].ProductId)).Data;
                if (stock != null)
                {
                    stock.Count -= @event.OrderItems[index].Count;
                    await _stockService.Update(new() {  Count = stock.Count, ProductId = @event.OrderItems[index].ProductId });
                }
            }

            _logger.LogInformation($"Stock was reserved for Buyer Id: {@event.BuyerId}");

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQConstant.StockReservedQueueName}"));

            var stockReservedEvent = new StockReservedEvent
            {
                BuyerId = @event.BuyerId,
                OrderId = @event.OrderId,
                OrderItems = @event.OrderItems,
                Payment = @event.Payment,
            };

            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            var stockNotReserved = new StockNotReservedEvent
            {
                Message = "Not enough stock!",
                OrderId = @event.OrderId
            };

            _logger.LogError($"Stock was not reserved for Order Id: {@event.BuyerId}");

            await _publishEndpoint.Publish(stockNotReserved);
        }
    }
}