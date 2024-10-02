using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class InternalProjectDto
    {
        
        public int? InternalProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
