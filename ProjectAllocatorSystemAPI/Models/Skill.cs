using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        public string SkillName { get; set; }

        public ICollection<EmployeeSkills> EmployeeSkills { get; set; }
    }
}
