using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ScheduleAPI.Models;
using System.Text;
using System.Text.Json;

namespace ScheduleAPI.Services
{
    public class RabbitMqMessageService : IMessageService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "orders";

        public RabbitMqMessageService(string connectionString)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // 確保隊列存在
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,  // 隊列持久化，在RabbitMQ重啟後保留
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public Task PushOrderAsync(Order order)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));
            
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // 消息持久化
            
            _channel.BasicPublish(
                exchange: "",
                routingKey: QueueName,
                basicProperties: properties,
                body: body);
            
            return Task.CompletedTask;
        }

        public Task<Order?> PopOrderAsync()
        {
            var result = _channel.BasicGet(QueueName, true);
            if (result == null)
            {
                return Task.FromResult<Order?>(null);
            }

            var message = Encoding.UTF8.GetString(result.Body.ToArray());
            var order = JsonSerializer.Deserialize<Order>(message);
            
            return Task.FromResult(order);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
} 