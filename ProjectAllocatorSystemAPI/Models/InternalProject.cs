using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class InternalProject
    {
        [Key]
        public int InternalProjectId {  get; set; }
        public string Name {  get; set; }
        public string Description { get; set; }
    }
}
