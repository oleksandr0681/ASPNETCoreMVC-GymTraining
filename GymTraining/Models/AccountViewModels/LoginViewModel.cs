// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace GymTraining.Models.AccountViewModels;

public class LoginViewModel
{
    //[Required]
    //[EmailAddress]
    //public string Email { get; set; }

    [Required]
    [Display(Name = "Ім'я користувача")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Display(Name = "Запам'ятати мене.")]
    public bool RememberMe { get; set; }
}
