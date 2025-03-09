using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models;
using ScheduleAPI.Services;

namespace ScheduleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IMessageService messageService, ILogger<OrderController> logger)
        : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("訂單資料不能為空");
            }

            logger.LogInformation("接收到訂單: {OrderId}, 金額: {Amount}", order.OrderId, order.Amount);
            
            await messageService.PushOrderAsync(order);
            
            return Ok(new { message = "訂單已成功提交", order });
        }
    }
} 