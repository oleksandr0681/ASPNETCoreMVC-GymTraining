using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GymTraining.Models
{
    [Display(Name = "Тренування")]
    public class Training
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Спортсмен")]
        public int SportsmanDataId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public SportsmanData? SportsmanData { get; set; } // Зв'язок з таблицею SportsmenData.

        //[Display(Name = "Тренер")]
        //public int TrainerDataId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        //public TrainerData? TrainerData { get; set; } // Зв'язок з таблицею TrainersData.

        [Display(Name = "Дата й час тренування")]
        public DateTime TrainingStartTime { get; set; } = DateTime.Now;

        [Display(Name = "Фізична вправа")]
        public int ExerciseId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public Exercise? Exercise { get; set; } // Зв'язок з таблицею Exercises.

        [Display(Name = "Харчування")]
        public string? Meal { get; set; }

        [Display(Name = "Виконано")]
        public bool IsCompleted { get; set; } = false;
    }
}
