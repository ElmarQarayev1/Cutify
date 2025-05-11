using System;
using Cutify.Models;

namespace Cutify.Repositories
{
	public interface ICreateReservations
	{
        Task<List<User>> GetAllBarbersWithWorkplaceAsync();
        Task<User> GetBarberByIdAsync(string barberId);
        Task<Reservation> GetReservationByIdAsync(int id);
        Task AddReservationAsync(Reservation reservation);
        Task SaveChangesAsync();
    }
}

