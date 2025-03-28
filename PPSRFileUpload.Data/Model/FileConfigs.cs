using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class FileConfig 
    {
        public Guid Id { get; set; }
        
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        
        [MaxLength(100)]
        public string? MiddleName { get; set; }

        
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }

        
        [MaxLength(17)]
        [Required]
        public string Vin { get; set; }

        
        [MaxLength(450)]
        [Required]
        public DateTime Startdate { get; set; }

        [Required]
        public DurationEnum Duration { get; set; }

        
        [MaxLength(50)]
        [Required]
        public string SpgAcn { get; set; }

        
        [MaxLength(100)]
        [Required]
        public string SpgOrganization { get; set; }
    }
}