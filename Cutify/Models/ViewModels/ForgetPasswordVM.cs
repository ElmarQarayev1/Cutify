using System;
using System.ComponentModel.DataAnnotations;

namespace Cutify.Models.ViewModels
{
	public class ForgetPasswordVM
	{
        [Required(ErrorMessage = "E-poçt boş ola bilməz.")]
        [EmailAddress(ErrorMessage = "E-poçt formatı düzgün deyil.")]
        [MaxLength(100)]
        [MinLength(3, ErrorMessage = "E-poçt çox qısadır.")]
        public string Email { get; set; }
    }
}

