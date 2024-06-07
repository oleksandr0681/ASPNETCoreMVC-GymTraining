using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GymTraining.Models
{
    [Index(nameof(Name), IsUnique = true), Display(Name = "Вправа")]
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150), Display(Name = "Назва вправи")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000), Display(Name = "Опис")]
        public string? Description { get; set; } = string.Empty;
    }
}
