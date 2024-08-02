using System.ComponentModel.DataAnnotations;

namespace ReflectionAttributePart2Demo.Models
{
    public class Reward
    {
        [Required]
        public int Id { get; set; }

        [StringLength(32, MinimumLength = 1)]
        [Display(Name = "Reward Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(128, MinimumLength = 1)]
        [Display(Name = "Reward Description")]
        public string Description { get; set; } = string.Empty;
    }
}
