using System;
using Cutify.Models;

namespace Cutify.Repositories
{
	public interface IReservationRepository
	{
        Task<List<Reservation>> GetReservationsByBarberIdAsync(string barberId, DateTime? date);
    }
}



