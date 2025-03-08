using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models;
using ScheduleAPI.Services;

namespace ScheduleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMessageService messageService, ILogger<OrderController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("訂單資料不能為空");
            }

            _logger.LogInformation("接收到訂單: {OrderId}, 金額: {Amount}", order.OrderId, order.Amount);
            
            await _messageService.PushOrderAsync(order);
            
            return Ok(new { Message = "訂單已提交" });
        }
    }
} 