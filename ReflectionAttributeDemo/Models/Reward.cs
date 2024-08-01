using System.ComponentModel.DataAnnotations;

namespace ReflectionAttributeDemo.Models
{
    public class Reward
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Reward Name")]
        [StringLength(32, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Reward Description")]
        [StringLength(128, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;
    }
}
