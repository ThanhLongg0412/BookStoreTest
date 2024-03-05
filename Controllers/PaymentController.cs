using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using bookstore.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentModel paymentModel;

        public PaymentController(PaymentModel paymentModel)
        {
            this.paymentModel = paymentModel;
        }

        [HttpGet]
        public IActionResult GetAllPaymentMethods()
        {
            var paymentMethods = paymentModel.GetAllPaymentMethods();
            return Ok(paymentMethods);
        }

        [HttpGet("{id}")]
        public IActionResult GetPaymentMethodById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            var paymentMethod = paymentModel.GetPaymentMethodById(id);

            if (paymentMethod == null)
            {
                return NotFound("Payment method not found.");
            }

            return Ok(paymentMethod);
        }

        [HttpPost]
        public IActionResult AddPaymentMethod([FromBody] PaymentMethod paymentMethod)
        {
            if (paymentMethod == null || string.IsNullOrEmpty(paymentMethod.Name))
            {
                return BadRequest("Payment method name is required.");
            }

            if (paymentModel.AddPaymentMethod(paymentMethod))
            {
                return Ok("Payment method added successfully.");
            }
            else
            {
                return BadRequest("Failed to add payment method.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePaymentMethod(int id, [FromBody] PaymentMethod paymentMethod)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            if (paymentMethod == null || string.IsNullOrEmpty(paymentMethod.Name))
            {
                return BadRequest("Payment method name is required.");
            }

            paymentMethod.Id = id;
            if (paymentModel.UpdatePaymentMethod(paymentMethod))
            {
                return Ok("Payment method updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update payment method.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePaymentMethod(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            if (paymentModel.DeletePaymentMethod(id))
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
