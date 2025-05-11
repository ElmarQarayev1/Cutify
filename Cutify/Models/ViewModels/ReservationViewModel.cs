using System;
using System.ComponentModel.DataAnnotations;

namespace Cutify.Models.ViewModels
{
	public class ReservationViewModel
	{

        public string CustomerFullName { get; set; }

        [Required(ErrorMessage = "Telefon nömrəsi tələb olunur.")]
        [Phone(ErrorMessage = "Telefon nömrəsi düzgün deyil.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Telefon nömrəsi ən azı 10 simvoldan ibarət olmalıdır.")]
        public string CustomerPhone { get; set; }

        public string BarberId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string TimeInput { get; set; }

    }
}

