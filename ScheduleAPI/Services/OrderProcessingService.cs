using ScheduleAPI.Models;

namespace ScheduleAPI.Services
{
    public class OrderProcessingService(IMessageService messageService, IProcessingLogService logService, ILogger<OrderProcessingService> logger)
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
                        var message = "本次起床沒有處理到任何訂單";
                        logger.LogInformation(message);
                        await logService.AddLogAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"訂單處理時發生錯誤: {ex.Message}";
                    logger.LogError(ex, errorMessage);
                    await logService.AddLogAsync(errorMessage);
                }

                await Task.Delay(_processInterval, stoppingToken);
            }
        }

        private async Task ProcessOrderAsync(Order order)
        {
            var message = $"處理訂單: {order.OrderId}, 金額: {order.Amount}";
            logger.LogInformation(message);
            await logService.AddLogAsync(message);
            // 在這裡可以添加更多訂單處理邏輯
        }
    }
} 