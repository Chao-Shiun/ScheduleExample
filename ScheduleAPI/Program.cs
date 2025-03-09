using ScheduleAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// 添加 SignalR 服務
builder.Services.AddSignalR();

// 註冊處理日誌服務
builder.Services.AddSingleton<IProcessingLogService, ProcessingLogService>();

// 根據環境決定使用的訊息服務
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"目前執行環境: {environment}");

// 註冊消息服務工廠
builder.Services.AddSingleton<MessageServiceFactory>();

// 註冊消息服務，使用工廠模式創建
builder.Services.AddSingleton<IMessageService>(provider => 
{
    var factory = provider.GetRequiredService<MessageServiceFactory>();
    
    if (builder.Environment.IsDevelopment())
    {
        // 開發環境使用 Redis
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
        Console.WriteLine($"開發環境使用 Redis: {redisConnectionString}");
        return factory.CreateRedisMessageService(redisConnectionString);
    }
    else
    {
        // 生產環境使用 RabbitMQ
        var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? "localhost";
        Console.WriteLine($"生產環境使用 RabbitMQ: {rabbitMqConnectionString}");
        return factory.CreateRabbitMQMessageService(rabbitMqConnectionString);
    }
});

// 添加背景服務
builder.Services.AddHostedService<OrderProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 配置靜態文件
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

// 配置 SignalR Hub
app.MapHub<OrderHub>("/orderhub");

app.MapControllers();

app.Run();
