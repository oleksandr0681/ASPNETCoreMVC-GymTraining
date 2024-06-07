using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GymTraining.Models
{
    [Index(nameof(ApplicationUserId), IsUnique = true), Display(Name = "Тренер")]
    public class TrainerData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string ApplicationUserId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public ApplicationUser? ApplicationUser { get; set; } // Зв'язок з таблицею AspNetUsers

        [MaxLength(200), Display(Name = "Прізвище, ім'я, по батькові")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Тренуємі спортсмени")]
        public ICollection<SportsmanData>? SportsmenData { get; set; }
    }
}
