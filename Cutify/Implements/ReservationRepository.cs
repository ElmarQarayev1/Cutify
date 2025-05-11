using System;
using Cutify.DATA;
using Cutify.Models;
using Cutify.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Implements
{
	public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext_ _context;

        public ReservationRepository(AppDbContext_ context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetReservationsByBarberIdAsync(string barberId, DateTime? date)
        {
            var query = _context.Reservations
                                .Where(r => r.BarberId == barberId);

            DateTime targetDate = date ?? DateTime.Now;

            query = query.Where(r => r.ReservationDate.Date == targetDate.Date);

            return await query.ToListAsync();
        }
    }
}


