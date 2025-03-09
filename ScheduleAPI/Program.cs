using ScheduleAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// 從配置中獲取 RabbitMQ 連接字符串
var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? "localhost";

// 註冊消息服務工廠
builder.Services.AddSingleton<MessageServiceFactory>();

// 註冊消息服務，使用工廠模式創建
builder.Services.AddSingleton<IMessageService>(provider => 
{
    var factory = provider.GetRequiredService<MessageServiceFactory>();
    return factory.CreateMessageService(rabbitMqConnectionString);
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
app.MapControllers();

app.Run();
