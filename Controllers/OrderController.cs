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
        public IActionResult AddOrder([FromBody] Order order)
        {
            if (string.IsNullOrEmpty(order.Status))
            {
                return BadRequest("Order status is required.");
            }

            if (order.OrderDate == default)
            {
                return BadRequest("Order date is required.");
            }

            if (order.PaymentMethodId == 0)
            {
                return BadRequest("Order payment method id is required.");
            }

            if (order.AdminId == 0)
            {
                return BadRequest("Order admin id is required.");
            }

            if (order.CustomerId == 0)
            {
                return BadRequest("Order customer id is required.");
            }

            if (_orderModel.AddOrder(order))
            {
                return Ok("Order added successfully.");
            }
            else
            {
                return BadRequest("Failed to add order.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order order)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            if (string.IsNullOrEmpty(order.Status))
            {
                return BadRequest("Order status is required.");
            }

            if (order.OrderDate == default)
            {
                return BadRequest("Order date is required.");
            }

            if (order.PaymentMethodId == 0)
            {
                return BadRequest("Order payment method id is required.");
            }

            if (order.AdminId == 0)
            {
                return BadRequest("Order admin id is required.");
            }

            if (order.CustomerId == 0)
            {
                return BadRequest("Order customer id is required.");
            }

            if (_orderModel.UpdateOrder(id, order))
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
