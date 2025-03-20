using System;
using System.Text.Json;
using eTickets.Data.Cart;
using eTickets.Libraries;
using eTickets.Models.VNPay;
using eTickets.Services.VNPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace eTickets.Controllers
{
    public class VNPayController : Controller
    {

        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;
        public VNPayController(IVnPayService vnPayService, IConfiguration configuration)
        {

            _vnPayService = vnPayService;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            //var url_01 = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?\r\nvnp_Amount=10000000\r\n&vnp_Command=pay\r\n&vnp_CreateDate=20250203105636\r\n&vnp_CurrCode=VND\r\n&vnp_IpAddr=127.0.0.1\r\n&vnp_Locale=vn\r\n&vnp_OrderInfo=Thanh+toan+don+hang+12345\r\n&vnp_OrderType=billpayment\r\n&vnp_ReturnUrl=https%3A%2F%2Flocalhost%3A44366%2FVNPay%2FPaymentCallbackVnpay\r\n&vnp_TmnCode=WSO4URPZ\r\n&vnp_TxnRef=202502031056365939\r\n&vnp_Version=2.1.0\r\n&vnp_SecureHash=5aff64fbb14da5f0d867a2633c1721cc0ddc9ea166671223b39c0a8043a0b9395497c5c5fd4e85a0a80b1375459f90b8a6ffb0b342dfd687ae5face06eae752f";
            return Redirect(url);
        }


        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }

    }
}
