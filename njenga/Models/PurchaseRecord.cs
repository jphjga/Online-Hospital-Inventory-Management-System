using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Njenga.Models
{
    public class PurchaseRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;       
        public string? Amount { get; set; }

        [Required]
        public int? Quantity { get; set; }       
        public decimal? Price { get; set; }
        public DateTime Date_Time { get; set; }
        [Required]
        public int InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public Institution? Institution { get; set; }
    }
}
