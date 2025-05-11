using System;
using System.ComponentModel.DataAnnotations;

namespace Cutify.Models.ViewModels
{
	public class RegisterVM
	{
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string WorkPlaceAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}

