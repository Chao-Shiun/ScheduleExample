using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models;
using ScheduleAPI.Services;

namespace ScheduleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(RedisService redisService, ILogger<OrderController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (string.IsNullOrEmpty(order.OrderId))
            {
                return BadRequest("訂單ID不能為空");
            }

            if (order.Amount <= 0)
            {
                return BadRequest("金額必須大於零");
            }

            _logger.LogInformation("接收到新訂單: {OrderId}, 金額: {Amount}", order.OrderId, order.Amount);
            
            await _redisService.PushOrderAsync(order);
            
            return Ok(new { message = "訂單已成功提交", order });
        }
    }
} 