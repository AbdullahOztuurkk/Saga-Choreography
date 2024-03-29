using Application.Services.Abstract;
using Application.Services.Concrete;
using Domain.Concrete;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace Application.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedEventConsumer> _logger;
    private readonly IStockService _stockService;
    private readonly IPublishEndpoint _publishEndpoint;
    public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, IPublishEndpoint publishEndpoint, IStockService stockService)
    {
        this._logger = logger;
        this._publishEndpoint = publishEndpoint;
        this._stockService = stockService;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var @event = context.Message;
        for(int index = 0; index < @event.OrderItems.Count; index++)
        {
            var stock = (await _stockService.GetByProductId(@event.OrderItems[index].ProductId)).Data;
            if(stock != null) 
            {
                stock.Count += @event.OrderItems[index].Count;
                await _stockService.Update(new() { Count = stock.Count, ProductId = @event.OrderItems[index].ProductId });
            }
        }

        _logger.LogInformation($"Stock was released for Order Id ({@event.OrderId})");
    }
}
