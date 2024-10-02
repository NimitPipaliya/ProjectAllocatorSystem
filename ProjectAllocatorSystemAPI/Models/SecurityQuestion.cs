using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class SecurityQuestion
    {
        [Key]
        public int SecurityQuestionId { get; set; }
        public string SecurityQuestionName { get; set; }
    }
}
