using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Njenga.Models
{
    public class Product
    {
        [Key]
        public int Product_id { get; set; }

        [Required]
        public int InstitutionId { get; set; } // Foreign key to Institutions table

        [ForeignKey("InstitutionId")]
        public Institution? Institution { get; set; } // Make nullable if not always set

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty; // Default value

        [StringLength(500)]
        public string Description { get; set; } = string.Empty; // Default value

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime? Expiry { get; set; }

        [StringLength(255)]
        public string Barcode { get; set; } = string.Empty; // Default value

        [Range(0, int.MaxValue)]
        public int? Quantity_alert { get; set; }

        [StringLength(255)]
        public string Category { get; set; } = string.Empty; // Default value
    }
}
