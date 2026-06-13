using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductOrdersController : ControllerBase
    {
        private readonly IProductOrderService _service;
        private readonly ILogger<ProductOrdersController> _logger;

        public ProductOrdersController(
            IProductOrderService service,
            ILogger<ProductOrdersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductOrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _service.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                return StatusCode(500, new { message = "An error occurred while fetching orders" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductOrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _service.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order");
                return StatusCode(500, new { message = "An error occurred while fetching the order" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductOrderDto>> CreateOrder([FromBody] CreateProductOrderRequest request)
        {
            try
            {
                var order = await _service.CreateOrderAsync(request);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "An error occurred while creating the order" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductOrderDto>> UpdateOrder(int id, [FromBody] UpdateProductOrderRequest request)
        {
            try
            {
                var order = await _service.UpdateOrderAsync(id, request);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return StatusCode(500, new { message = "An error occurred while updating the order" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _service.DeleteOrderAsync(id);
                if (!result)
                    return NotFound(new { message = "Order not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");
                return StatusCode(500, new { message = "An error occurred while deleting the order" });
            }
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<List<ProductOrderDto>>> GetOrdersByStatus(string status)
        {
            try
            {
                var orders = await _service.GetOrdersByStatusAsync(status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders by status");
                return StatusCode(500, new { message = "An error occurred while fetching orders" });
            }
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<List<ProductOrderDto>>> GetOrdersByUser(int userId)
        {
            try
            {
                var orders = await _service.GetOrdersByUserAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user orders");
                return StatusCode(500, new { message = "An error occurred while fetching orders" });
            }
        }

        [HttpGet("stats/overview")]
        public async Task<ActionResult<OrderStatisticsDto>> GetStatistics()
        {
            try
            {
                var stats = await _service.GetStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching statistics");
                return StatusCode(500, new { message = "An error occurred while fetching statistics" });
            }
        }

        [HttpGet("stats/monthly")]
        public async Task<ActionResult<List<MonthlyRevenueDto>>> GetMonthlyRevenue()
        {
            try
            {
                var data = await _service.GetMonthlyRevenueAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching monthly revenue");
                return StatusCode(500, new { message = "An error occurred while fetching revenue data" });
            }
        }
    }
}
