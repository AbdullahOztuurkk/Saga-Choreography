using Application.Consumers;
using MassTransit;
using SharedLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<StockReservedEventConsumer>();
    conf.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("host.docker.internal", (auth) =>
        {
            auth.Username("guest");
            auth.Password("guest");
        });
        cfg.ReceiveEndpoint(RabbitMQConstant.StockReservedQueueName, x =>
        {
            x.ConfigureConsumer<StockReservedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
