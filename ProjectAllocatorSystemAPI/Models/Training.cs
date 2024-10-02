using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class Training
    {
        [Key]
        public int TrainingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
