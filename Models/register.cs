using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace new1.Models
{
    public class register
    {
        [Key]
        [Required(ErrorMessage = "Email ID is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string emailId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string last_name { get; set; }
        [Required(ErrorMessage = "Age is required.")]
        public String Age { get; set; }
        [Required(ErrorMessage = "ID Number is required.")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Compare("password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string confirm_passwordone { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        [Display(Name = "Image")]
        public byte[] Imagefile { get; set; }
        public virtual home Home { get; set; }

        public virtual ICollection<contact_us> ContactUsHistory { get; set; }

    }
}
