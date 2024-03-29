using SharedLib.ValueObject;

namespace SharedLib;
public class PaymentSuccessEvent
{
    public long OrderId { get; set; }
    public string BuyerId { get; set; }
}

public class PaymentFailedEvent
{
    public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
    public long OrderId { get; set; }
    public string BuyerId { get; set; }
    public string Message { get; set; }
}
