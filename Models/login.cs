using System.ComponentModel.DataAnnotations;

namespace new1.Models
{
    public class login
    {
        [Required(ErrorMessage = "ID Number is required.")]
        public string emailId { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }  

    }
}
