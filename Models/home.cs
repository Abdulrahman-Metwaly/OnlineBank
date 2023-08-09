using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace new1.Models
{
    public class home
    {
        [Key]
        [ForeignKey("Register")]
        public string emailId { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public byte[] imagefile { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public virtual register Register { get; set; }
    }
}