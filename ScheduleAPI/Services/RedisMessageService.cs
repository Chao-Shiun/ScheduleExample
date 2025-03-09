using StackExchange.Redis;
using ScheduleAPI.Models;
using System.Text.Json;
using Order = ScheduleAPI.Models.Order;

namespace ScheduleAPI.Services
{
    public class RedisMessageService : IMessageService
    {
        private readonly IDatabase _database;
        private const string QueueKey = "orders";

        public RedisMessageService(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _database = redis.GetDatabase();
        }

        public async Task PushOrderAsync(Order order)
        {
            string orderJson = JsonSerializer.Serialize(order);
            await _database.ListLeftPushAsync(QueueKey, orderJson);
        }

        public async Task<Order?> PopOrderAsync()
        {
            var orderJson = await _database.ListRightPopAsync(QueueKey);
            if (!orderJson.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Order>(orderJson.ToString());
        }
    }
} 