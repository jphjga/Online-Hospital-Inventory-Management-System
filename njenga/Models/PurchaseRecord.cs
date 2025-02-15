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

        // Amount is optional (nullable)
        public string? Amount { get; set; }

        [Required]
        public int? Quantity { get; set; }

        // Price is optional (nullable), using decimal for currency values
        public decimal? Price { get; set; }

        [Required]
        public int InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public Institution? Institution { get; set; }
    }
}
