using System.ComponentModel.DataAnnotations;

namespace eTickets.Models
{
    public class EmailMarketingModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string Name { get; set; }
    }
}
