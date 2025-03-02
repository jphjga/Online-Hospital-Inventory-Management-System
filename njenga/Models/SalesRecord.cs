using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Njenga.Models
{
    public class SalesRecord
    {
        [Key]
        public int Id { get; set; }  // Auto-incremented primary key

        [Required]
        public int InstitutionId { get; set; }  // Foreign key to the Institutions table

        [ForeignKey("InstitutionId")]
        public Institution? Institution { get; set; }

        [Required]
        public DateTime Date_Time { get; set; }  // Date and time of the sale

        [Required]
        [StringLength(255)]
        public string User { get; set; } = string.Empty;  // Username of the person who made the sale

        [Required]
        public int Count { get; set; }  // Number of products sold in the sale
    }
}
