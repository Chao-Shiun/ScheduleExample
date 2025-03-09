namespace ScheduleAPI.Services
{
    /// <summary>
    /// 消息服務工廠，用於根據配置創建適當的消息服務實例
    /// </summary>
    public class MessageServiceFactory(IConfiguration configuration)
    {
        /// <summary>
        /// 創建 Redis 消息服務
        /// </summary>
        /// <param name="connectionString">Redis 連接字串</param>
        /// <returns>Redis 消息服務實例</returns>
        public IMessageService CreateRedisMessageService(string connectionString)
        {
            return new RedisMessageService(connectionString);
        }

        /// <summary>
        /// 創建 RabbitMQ 消息服務
        /// </summary>
        /// <param name="connectionString">RabbitMQ 連接字串</param>
        /// <returns>RabbitMQ 消息服務實例</returns>
        public IMessageService CreateRabbitMQMessageService(string connectionString)
        {
            return new RabbitMqMessageService(connectionString);
        }
    }
} 