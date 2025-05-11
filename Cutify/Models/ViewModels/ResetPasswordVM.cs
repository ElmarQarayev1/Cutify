using System;
using System.ComponentModel.DataAnnotations;

namespace Cutify.Models.ViewModels
{
    public class ResetPasswordVM
    {
        [Required(ErrorMessage = "Yeni şifrə tələb olunur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən azı 6 simvoldan ibarət olmalıdır")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Təsdiq yeni şifrə tələb olunur")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifrə və təsdiq şifrəsi eyni olmalıdır")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string Email { get; set; }
    }

}

