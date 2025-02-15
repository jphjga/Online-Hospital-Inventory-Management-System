using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Njenga.Models
{
    public class StockTaking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InstitutionId { get; set; } // Foreign key to Institutions table

        [ForeignKey("InstitutionId")]
        public Institution? Institution { get; set; }


        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string User { get; set; } = string.Empty;
    }
}
