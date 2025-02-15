using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Njenga.Models
{
    public class Institution
    {
        [Key]
        public int InstitutionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty; // Default value

        [StringLength(255)]
        public string Location { get; set; } = string.Empty; // Default value

        public ICollection<Product>? Products { get; set; } // Make nullable
    }
}
