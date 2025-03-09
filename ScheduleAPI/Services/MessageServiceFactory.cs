using Microsoft.Extensions.Configuration;

namespace ScheduleAPI.Services
{
    /// <summary>
    /// 消息服務工廠，用於根據配置創建適當的消息服務實例
    /// </summary>
    public class MessageServiceFactory
    {
        private readonly IConfiguration _configuration;

        public MessageServiceFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 根據配置創建消息服務
        /// </summary>
        /// <returns>消息服務實例</returns>
        public IMessageService CreateMessageService(string? connectionString = null)
        {
            // 如果沒有提供連接字符串，則從配置中獲取
            connectionString ??= _configuration.GetConnectionString("RabbitMQ") ?? "localhost";

            // 根據配置決定使用哪種消息服務
            var messageServiceType = _configuration.GetValue<string>("MessageService:Type")?.ToLower() ?? "rabbitmq";

            return messageServiceType switch
            {
                "rabbitmq" => new RabbitMqMessageService(connectionString),
                // 可以在這裡添加其他消息服務類型
                _ => new RabbitMqMessageService(connectionString) // 默認使用 RabbitMQ
            };
        }
    }
} 