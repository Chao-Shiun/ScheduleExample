using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Services;

namespace ScheduleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessingLogController(IProcessingLogService logService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] int count = 50)
        {
            var logs = await logService.GetRecentLogsAsync(count);
            return Ok(logs);
        }
    }
} 