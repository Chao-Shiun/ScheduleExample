using ScheduleAPI.Models;

namespace ScheduleAPI.Services
{
    /// <summary>
    /// 消息服務介面，遵循開放封閉原則 (OCP)
    /// 允許不同實現（Redis和RabbitMQ）而不修改使用它的客戶端代碼
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 將訂單推送到隊列
        /// </summary>
        /// <param name="order">訂單資訊</param>
        Task PushOrderAsync(Order order);

        /// <summary>
        /// 從隊列中獲取訂單
        /// </summary>
        /// <returns>訂單資訊，如果隊列為空則返回null</returns>
        Task<Order?> PopOrderAsync();
    }
} 