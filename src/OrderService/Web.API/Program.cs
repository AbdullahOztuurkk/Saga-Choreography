using Application.Consumers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreLib.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Configurations;
using Persistence;
using SharedLib;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new CoreModule(builder.Configuration)));
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new OrderModule(builder.Configuration)));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<PaymentFailedEventConsumer>();
    conf.AddConsumer<PaymentSuccessEventConsumer>();
    conf.AddConsumer<StockNotReservedConsumer>();

    conf.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("host.docker.internal", (auth) =>
        {
            auth.Username("guest");
            auth.Password("guest");
        });
        cfg.ReceiveEndpoint(RabbitMQConstant.StockNotReservedQueueName, x =>
        {
            x.ConfigureConsumer<StockNotReservedConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQConstant.PaymentSuccessQueueName, x =>
        {
            x.ConfigureConsumer<PaymentSuccessEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQConstant.PaymentFailedQueueName, x =>
        {
            x.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
    });

});

var app = builder.Build();

var context = app.Services.GetService<OrderDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    await context.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
