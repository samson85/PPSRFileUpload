
using System.ComponentModel.DataAnnotations;

namespace Model.FileModel
{
    public class ValidationError 
    {
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}