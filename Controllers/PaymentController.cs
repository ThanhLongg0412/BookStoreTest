﻿using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

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

        [HttpGet("{id}")]
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
        public IActionResult AddPaymentMethod([FromBody] string paymentMethodName)
        {
            if (string.IsNullOrEmpty(paymentMethodName))
            {
                return BadRequest("Payment method name is required.");
            }

            if (_paymentModel.AddPaymentMethod(paymentMethodName))
            {
                return Ok("Payment method added successfully.");
            }
            else
            {
                return BadRequest("Failed to add payment method.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePaymentMethod(int id, [FromBody] string paymentMethodName)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid payment method ID.");
            }

            if (string.IsNullOrEmpty(paymentMethodName))
            {
                return BadRequest("Payment method name is required.");
            }

            if (_paymentModel.UpdatePaymentMethod(id, paymentMethodName))
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
