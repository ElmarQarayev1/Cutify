using System;
using Microsoft.AspNetCore.Identity;

namespace Cutify.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
        public string FullName => $"{FirstName} {LastName}";
        
        public string? ImageUrl { get; set; }
        
        public string WorkPlaceAddress { get; set; }
      
        public ICollection<Reservation> Reservations { get; set; }
    }
}

