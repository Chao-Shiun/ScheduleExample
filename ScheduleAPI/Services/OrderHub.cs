using Microsoft.AspNetCore.SignalR;
using ScheduleAPI.Models;

namespace ScheduleAPI.Services
{
    public class OrderHub : Hub
    {
        // 此Hub用於訂單處理狀態的實時傳遞
        // SignalR會自動處理連接和通信
    }
    
    // 擴展服務，添加日誌推送功能
    public interface IProcessingLogService
    {
        Task AddLogAsync(string message);
        Task<List<ProcessingLog>> GetRecentLogsAsync(int count = 50);
    }
    
    public class ProcessingLogService(IHubContext<OrderHub> hubContext) : IProcessingLogService
    {
        private readonly List<ProcessingLog> _recentLogs = new(100); // 保留最近100條記錄
        private readonly Lock _logsLock = new();

        public async Task AddLogAsync(string message)
        {
            var log = new ProcessingLog
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = message
            };
            
            // 添加到內存中的日誌列表
            lock (_logsLock)
            {
                _recentLogs.Insert(0, log); // 新的日誌插入到列表頂部
                
                // 如果超過容量，移除最舊的日誌
                if (_recentLogs.Count > 100)
                {
                    _recentLogs.RemoveAt(_recentLogs.Count - 1);
                }
            }
            
            // 通過SignalR推送給所有客戶端
            await hubContext.Clients.All.SendAsync("ReceiveLog", log);
        }
        
        public Task<List<ProcessingLog>> GetRecentLogsAsync(int count = 50)
        {
            lock (_logsLock)
            {
                // 返回最近的日誌（已經是按新到舊排序）
                return Task.FromResult(_recentLogs.Take(Math.Min(count, _recentLogs.Count)).ToList());
            }
        }
    }
} 