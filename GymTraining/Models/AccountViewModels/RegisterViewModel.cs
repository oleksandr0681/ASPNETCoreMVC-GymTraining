// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace GymTraining.Models.AccountViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Потрібно вибрати роль.")]
    public string Role { get; set; }

    [Required]
    [Display(Name = "Ім'я користувача")]
    public string UserName { get; set; }

    [MaxLength(200), Display(Name = "Прізвище, ім'я, по батькові")]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердіть пароль")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
    public string ConfirmPassword { get; set; }
}
