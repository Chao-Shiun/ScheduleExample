using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScheduleAPI.Models;

namespace ScheduleAPI.Services
{
    public class OrderProcessingService : BackgroundService
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<OrderProcessingService> _logger;
        private readonly TimeSpan _processInterval = TimeSpan.FromSeconds(3);

        public OrderProcessingService(IMessageService messageService, ILogger<OrderProcessingService> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var order = await _messageService.PopOrderAsync();
                    if (order != null)
                    {
                        await ProcessOrderAsync(order);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "訂單處理時發生錯誤");
                }

                await Task.Delay(_processInterval, stoppingToken);
            }
        }

        private Task ProcessOrderAsync(Order order)
        {
            _logger.LogInformation("處理訂單: {OrderId}, 金額: {Amount}", order.OrderId, order.Amount);
            // 在這裡可以添加更多訂單處理邏輯
            return Task.CompletedTask;
        }
    }
} 