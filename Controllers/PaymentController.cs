using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentModel _paymentModel;

        public PaymentController(IConfiguration configuration)
        {
            _paymentModel = new PaymentModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllPaymentMethods()
        {
            var paymentMethods = _paymentModel.GetAllPaymentMethods();
            return Ok(paymentMethods);
        }

        [HttpGet("id={id}")]
        public IActionResult GetPaymentMethodById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            var paymentMethod = _paymentModel.GetPaymentMethodById(id);

            if (paymentMethod == null)
            {
                return NotFound("Payment method not found.");
            }

            return Ok(paymentMethod);
        }

        [HttpPost]
        public IActionResult AddPaymentMethod([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Payment method name is required.");
            }

            if (_paymentModel.AddPaymentMethod(name))
            {
                return Ok("Payment method added successfully.");
            }
            else
            {
                return BadRequest("Failed to add payment method.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdatePaymentMethod(int id, [FromBody] string name)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Payment method name is required.");
            }

            if (_paymentModel.UpdatePaymentMethod(id, name))
            {
                return Ok("Payment method updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update payment method.");
            }
        }

        [HttpDelete("id={id}")]
        public IActionResult DeletePaymentMethod(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            if (_paymentModel.DeletePaymentMethod(id))
            {
                return Ok("Payment method deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete payment method.");
            }
        }
    }
}
