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
                "redis" => new RedisMessageService(connectionString),
                // 可以在這裡添加其他消息服務類型
                _ => new RabbitMqMessageService(connectionString) // 默認使用 RabbitMQ
            };
        }

        /// <summary>
        /// 創建 Redis 消息服務
        /// </summary>
        /// <param name="connectionString">Redis 連接字符串</param>
        /// <returns>Redis 消息服務實例</returns>
        public IMessageService CreateRedisMessageService(string connectionString)
        {
            return new RedisMessageService(connectionString);
        }

        /// <summary>
        /// 創建 RabbitMQ 消息服務
        /// </summary>
        /// <param name="connectionString">RabbitMQ 連接字符串</param>
        /// <returns>RabbitMQ 消息服務實例</returns>
        public IMessageService CreateRabbitMQMessageService(string connectionString)
        {
            return new RabbitMqMessageService(connectionString);
        }
    }
} 