using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class ImportedFiles 
    {
        public Guid Id { get; set; }
        
        [MaxLength(100)]
        [Required]
        public string FileName { get; set; }       
        
    }
}