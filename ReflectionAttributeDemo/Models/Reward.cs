using System.ComponentModel.DataAnnotations;

namespace ReflectionAttributeDemo.Models
{
    public class Reward
    {
        [Required]
        public int Id { get; set; }

        [StringLength(32, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(128, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;
    }
}
