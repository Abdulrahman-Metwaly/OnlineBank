using System.ComponentModel.DataAnnotations;

namespace new1.Models
{
    public class Deposit
    {
        [Required(ErrorMessage = "Card Number is required.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration Date is required.")]
        public string ExpirationDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }
    }
}
