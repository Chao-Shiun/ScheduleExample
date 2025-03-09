using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ScheduleAPI.Models;
using System.Text;
using System.Text.Json;

namespace ScheduleAPI.Services
{
    public class RabbitMqMessageService : IMessageService, IAsyncDisposable
    {
        private readonly Task<IConnection> _connectionTask;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string QueueName = "orders";
        private bool _disposed = false;

        public RabbitMqMessageService(string connectionString)
        {
            var factory = new ConnectionFactory 
            {
                HostName = "localhost",  // RabbitMQ 伺服器位址
                UserName = "guest",      // 預設使用者
                Password = "guest",      // 預設密碼
                // 啟用自動恢復連接
                AutomaticRecoveryEnabled = true,
                // 嘗試恢復的間隔時間
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            
            // 使用異步方式創建連接
            _connectionTask = factory.CreateConnectionAsync();
            
            // 初始化通道和隊列
             InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            _connection = await _connectionTask;
            _channel = await _connection.CreateChannelAsync();
            
            // 確保隊列存在
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,  // 隊列持久化，在RabbitMQ重啟後保留
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public async Task PushOrderAsync(Order order)
        {
            if (_channel == null || _disposed)
            {
                throw new InvalidOperationException("RabbitMQ 通道未初始化或已釋放");
            }

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));
            
            // 創建消息屬性
            var props = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent, // 消息持久化
            };
            
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: false,
                basicProperties: props,
                body: body);
        }

        public async Task<Order?> PopOrderAsync()
        {
            if (_channel == null || _disposed)
            {
                throw new InvalidOperationException("RabbitMQ 通道未初始化或已釋放");
            }

            try
            {
                var result = await _channel.BasicGetAsync(QueueName, true);
                if (result == null)
                {
                    return null;
                }

                var message = Encoding.UTF8.GetString(result.Body.ToArray());
                var order = JsonSerializer.Deserialize<Order>(message);
                
                return order;
            }
            catch (Exception ex)
            {
                // 記錄錯誤並返回 null
                Console.WriteLine($"從 RabbitMQ 獲取消息時發生錯誤: {ex.Message}");
                return null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            
            _disposed = true;
            
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
            
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }

        // 為了向後兼容，保留同步 Dispose 方法
        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
} 