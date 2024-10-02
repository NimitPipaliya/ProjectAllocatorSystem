using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class TrainingDto
    {
        
        public int? TrainingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
