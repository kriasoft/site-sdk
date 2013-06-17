// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewUser.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterUser
    {
        [Required(ErrorMessage = "The 'Username' field is required")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The 'Password' field is required")]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "The 'Email' field is required")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "The 'Display Name' field is required")]
        [StringLength(50, MinimumLength = 2)]
        public string DisplayName { get; set; }
    }
}
