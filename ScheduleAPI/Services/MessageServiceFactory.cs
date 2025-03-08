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
        public IMessageService CreateMessageService()
        {
            var messageType = _configuration.GetValue<string>("MessageService:Type") ?? "Redis";
            
            return messageType.ToLower() switch
            {
                "redis" => new RedisMessageService(_configuration.GetValue<string>("MessageService:Redis:ConnectionString") ?? "localhost:6379"),
                "rabbitmq" => new RabbitMqMessageService(_configuration.GetValue<string>("MessageService:RabbitMQ:ConnectionString") ?? "amqp://guest:guest@localhost:5672"),
                _ => new RedisMessageService(_configuration.GetValue<string>("MessageService:Redis:ConnectionString") ?? "localhost:6379")
            };
        }
    }
} 