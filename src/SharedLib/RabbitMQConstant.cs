namespace SharedLib;
public class RabbitMQConstant
{
    public const string StockOrderCreatedQueueName = "stock-order-created-queue";
    public const string StockReservedQueueName = "stock-reserved-queue";
    public const string PaymentSuccessQueueName = "payment-success-queue";
    public const string PaymentFailedQueueName  = "payment-failed-queue";
    public const string StockNotReservedQueueName = "stock-not-reserved-queue";
}
