// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewUser.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginUser
    {
        [Required(ErrorMessage = "The 'Username' field is required"), StringLength(100)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The 'Password' field is required"), StringLength(50)]
        public string Password { get; set; }
    }
}
