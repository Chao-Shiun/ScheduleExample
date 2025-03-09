using StackExchange.Redis;
using System.Text.Json;

namespace ScheduleAPI.Services
{
    public class RedisService
    {
        private readonly IDatabase _database;
        private const string QueueKey = "orders";

        public RedisService(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _database = redis.GetDatabase();
        }

        public async Task PushOrderAsync(Models.Order order)
        {
            string orderJson = JsonSerializer.Serialize(order);
            await _database.ListLeftPushAsync(QueueKey, orderJson);
        }

        public async Task<Models.Order?> PopOrderAsync()
        {
            var orderJson = await _database.ListRightPopAsync(QueueKey);
            if (!orderJson.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Models.Order>(orderJson.ToString());
        }
    }
} 