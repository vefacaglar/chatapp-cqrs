using ChatApp.Application.Chat;
using ChatApp.Application.EventHandler;
using ChatApp.Application.Middleware;
using ChatApp.Domain;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using ChatApp.Application.EventBus;
using ChatApp.Infrastructure.Database.Command;
using CustomDispatcher;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlServer(configuration["ConnectionStrings:ChatDbCommand"]));
builder.Services.AddSingleton(configuration.Get<ChatAppConfiguration>()!);

builder.Services.AddDomain();
builder.Services.AddInfrastructure();

builder.Services.AddCustomDispatcher(options =>
{
    options.RegisterServicesFromAssembly(typeof(CreateChatRoomCommand).Assembly);
    options.AddDispatchMiddleware(typeof(LoggingDispatchMiddleware<,>));
});

builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
builder.Services.AddTransient<IEventHandler<EventCreatedChatRoom>, CreatedChatRoomEventHandler>();

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var config = sp.GetRequiredService<ChatAppConfiguration>();
    return new ConnectionFactory
    {
        HostName = config.ConnectionStrings.EventBus.Connection,
        UserName = config.ConnectionStrings.EventBus.UserName,
        Password = config.ConnectionStrings.EventBus.Password
    };
});

builder.Services.AddSingleton<IPersistentConnection<IChannel>, RabbitMQPersistentConnection>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    var persistentConnection = sp.GetRequiredService<IPersistentConnection<IChannel>>();
    var eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
    var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
    var config = sp.GetRequiredService<ChatAppConfiguration>();
    return new RabbitMQEventBus(eventDispatcher, persistentConnection, logger, config.RetryCount);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ConfigureEventBus(app);

app.Run();

void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<EventCreatedChatRoom>();
}
