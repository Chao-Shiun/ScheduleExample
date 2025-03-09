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
        /// 將訂單推送到消息隊列
        /// </summary>
        /// <param name="order">要推送的訂單</param>
        Task PushOrderAsync(Order order);

        /// <summary>
        /// 從消息隊列中獲取一個訂單
        /// </summary>
        /// <returns>訂單對象，如果隊列為空則返回 null</returns>
        Task<Order?> PopOrderAsync();
    }
} 