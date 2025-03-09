using ScheduleAPI.Models;

namespace ScheduleAPI.Services
{
    public class OrderProcessingService(IMessageService messageService, ILogger<OrderProcessingService> logger)
        : BackgroundService
    {
        private readonly TimeSpan _processInterval = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var order = await messageService.PopOrderAsync();
                    if (order != null)
                    {
                        await ProcessOrderAsync(order);
                    }
                    else
                    {
                        logger.LogInformation("本次起床沒有處理到任何訂單");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "訂單處理時發生錯誤");
                }

                await Task.Delay(_processInterval, stoppingToken);
            }
        }

        private Task ProcessOrderAsync(Order order)
        {
            logger.LogInformation("處理訂單: {OrderId}, 金額: {Amount}", order.OrderId, order.Amount);
            // 在這裡可以添加更多訂單處理邏輯
            return Task.CompletedTask;
        }
    }
} 