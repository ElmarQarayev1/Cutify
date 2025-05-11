using System;
namespace Cutify.Models
{
    using System;

    public class Reservation
    {
        public int Id { get; set; }      
        public string CustomerFullName { get; set; }
        public string CustomerPhone { get; set; }
        public string BarberId { get; set; }
        public User Barber { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}

