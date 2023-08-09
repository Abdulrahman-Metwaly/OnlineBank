using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace new1.Models
{
    public class contact_us
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [ForeignKey("register")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string emailId { get; set; }
        public register Register { get; set; }


        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; }


        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }

        public string Answer { get; set; } = "Not Answered Yet";
    }
}
