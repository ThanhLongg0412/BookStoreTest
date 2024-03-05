using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderModel _orderModel;

        public OrderController(IConfiguration configuration)
        {
            _orderModel = new OrderModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderModel.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("id={id}")]
        public IActionResult GetOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            var order = _orderModel.GetOrderById(id);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(order);
        }

        [HttpPost]
        public IActionResult AddOrder([FromBody] string status, [FromBody] DateTime order_date,
            [FromBody] int payment_method_id, [FromBody] int admin_id, [FromBody] int customer_id)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Order status is required.");
            }

            if (order_date == default)
            {
                return BadRequest("Order date is required.");
            }

            if (payment_method_id == 0)
            {
                return BadRequest("Order payment method id is required.");
            }

            if (admin_id == 0)
            {
                return BadRequest("Order admin id is required.");
            }

            if (customer_id == 0)
            {
                return BadRequest("Order customer id is required.");
            }

            if (_orderModel.AddOrder(status, order_date, payment_method_id, admin_id, customer_id))
            {
                return Ok("Order added successfully.");
            }
            else
            {
                return BadRequest("Failed to add order.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateOrder(int id, [FromBody] string status, 
            [FromBody] DateTime order_date, [FromBody] int payment_method_id, 
            [FromBody] int admin_id, [FromBody] int customer_id)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Order status is required.");
            }

            if (order_date == default)
            {
                return BadRequest("Order date is required.");
            }

            if (payment_method_id == 0)
            {
                return BadRequest("Order payment method id is required.");
            }

            if (admin_id == 0)
            {
                return BadRequest("Order admin id is required.");
            }

            if (customer_id == 0)
            {
                return BadRequest("Order customer id is required.");
            }

            if (_orderModel.UpdateOrder(id, status, order_date, payment_method_id, admin_id, 
                customer_id))
            {
                return Ok("Order updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update order.");
            }
        }

        [HttpDelete("id={id}")]
        public IActionResult DeleteOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            if (_orderModel.DeleteOrder(id))
            {
                return Ok("Order deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete order.");
            }
        }
    }
}
