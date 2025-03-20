using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using eTickets.Libraries;
using eTickets.Models.VNPay;

namespace eTickets.Services.VNPay
{
    public class VNPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VNPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneId = _configuration["TimeZoneId"] ?? "Asia/Ho_Chi_Minh";
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            var pay = new VNPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            // Kiểm tra giá trị hợp lệ
            //if (model.Amount < 1000)
            //{
            //    throw new Exception("Số tiền thanh toán phải từ 1.000 VND trở lên.");
            //}

            // Thêm thông tin quan trọng
            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (100000 * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", "vn");

            // Đảm bảo không có ký tự đặc biệt
pay.AddRequestData("vnp_OrderInfo", Uri.EscapeDataString($"{model.Name} - {model.OrderDescription} - {100000} VND"));            
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);

            // Mã giao dịch hợp lệ
            pay.AddRequestData("vnp_TxnRef", timeNow.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999));

            if (!string.IsNullOrEmpty(_configuration["Vnpay:BankCode"]))
            {
                pay.AddRequestData("vnp_BankCode", _configuration["Vnpay:BankCode"]);
            }

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            // Kiểm tra log
            Console.WriteLine("🔍 VNPay URL: " + paymentUrl);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VNPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            return response;
        }
    }
}
