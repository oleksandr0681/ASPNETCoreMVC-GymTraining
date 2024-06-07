using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GymTraining.Models
{
    [Index(nameof(ApplicationUserId), IsUnique = true), Display(Name = "Спортсмен")]
    public class SportsmanData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string ApplicationUserId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public ApplicationUser? ApplicationUser { get; set; } // Зв'язок з таблицею AspNetUsers

        [MaxLength(200), Display(Name = "Прізвище, ім'я, по батькові")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Тренер")]
        public int TrainerDataId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public TrainerData? TrainerData { get; set; } // Зв'язок з таблицею TrainersData.

        [Display(Name = "Розклад тренувань")]
        public ICollection<Training>? Training { get; set; }
    }
}
