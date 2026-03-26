using FlowerShop.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API.Controllers
{
    public class PaymentsController(IFacadeService facadeService) : ODataController
    {
        private readonly IFacadeService _facadeService = facadeService;

        // GET api/Payments/{orderID}
        [HttpGet("api/Payments/{orderID}")]
        public async Task<IActionResult> GetByOrder([FromRoute] Guid orderID)
        {
            var result = await _facadeService.PaymentService.GetPaymentByOrderIDAsync(orderID);
            return Ok(result);
        }

        // POST api/Payments/COD
        [HttpPost("api/Payments/COD")]
        public async Task<IActionResult> CreateCOD([FromQuery] Guid orderID, [FromQuery] decimal amount)
        {
            var result = await _facadeService.PaymentService.CreateCODPaymentAsync(orderID, amount);
            return Ok(result);
        }

        // POST api/Payments/PayOS
        [HttpPost("api/Payments/PayOS")]
        public async Task<IActionResult> CreatePayOS(
            [FromQuery] Guid orderID,
            [FromQuery] decimal amount,
            [FromQuery] string returnUrl,
            [FromQuery] string cancelUrl)
        {
            var result = await _facadeService.PaymentService.CreatePayOSPaymentAsync(orderID, amount, returnUrl, cancelUrl);
            return Ok(result);
        }

        // POST api/Payments/Webhook
        // AllowAnonymous vì PayOS gọi từ server ngoài, không có JWT
        [HttpPost("api/Payments/Webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] PayOSWebhookDTO webhook)
        {
            var result = await _facadeService.PaymentService.HandlePayOSWebhookAsync(webhook);
            return Ok(result);
        }

        // PUT api/Payments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] PaymentUpdateDTO dto)
        {
            var result = await _facadeService.PaymentService.UpdatePaymentStatusAsync(id, dto);
            return Ok(result);
        }
    }
}
