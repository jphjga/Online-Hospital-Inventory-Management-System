using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Njenga.Models
{
    public class Account
    {
        [Key]
        public int User_id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(255)]
        public required string Password { get; set; }

        [Required]
        public int InstitutionId { get; set; } // Foreign key to Institution

        [ForeignKey("InstitutionId")]
        public Institution? Institution { get; set; }
    }

}
