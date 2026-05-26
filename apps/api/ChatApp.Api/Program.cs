using ChatApp.Application.Chat;
using ChatApp.Application.EventHandler;
using ChatApp.Application.Middleware;
using ChatApp.Application.Services;
using ChatApp.Domain;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using ChatApp.Application.EventBus;
using ChatApp.Infrastructure.Database.Command;
using CustomDispatcher;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ChatDbContext>(options => options.UseNpgsql(configuration["ConnectionStrings:ChatDbCommand"]));
builder.Services.AddSingleton(configuration.Get<ChatAppConfiguration>()!);

builder.Services.AddDomain();
builder.Services.AddInfrastructure();

builder.Services.AddCustomDispatcher(options =>
{
    options.RegisterServicesFromAssembly(typeof(CreateChatRoomCommand).Assembly);
    options.AddDispatchMiddleware(typeof(LoggingDispatchMiddleware<,>));
});

builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
builder.Services.AddTransient<IEventHandler<EventCreatedChatRoom>, CreatedChatRoomEventHandler>();
builder.Services.AddTransient<IEventHandler<MessageSentEvent>, SentMessageEventHandler>();

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

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<ChatAppConfiguration>();
    return ConnectionMultiplexer.Connect(config.Redis.ConnectionString);
});

builder.Services.AddSingleton<IRedisPublisher, RedisPublisher>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("ChatApp API")
               .WithTheme(ScalarTheme.BluePlanet)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

ConfigureEventBus(app);

app.Run();

void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<EventCreatedChatRoom>();
    eventBus.Subscribe<MessageSentEvent>();
}
