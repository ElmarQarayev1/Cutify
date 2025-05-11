using System;
using Cutify.DATA;
using Cutify.Models;
using Cutify.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Implements
{
    public class CreateReservation : ICreateReservations
    {
        private readonly AppDbContext_ _context;

        public CreateReservation(AppDbContext_ context)
        {
            _context = context;
        }

      
        public async Task<List<User>> GetAllBarbersWithWorkplaceAsync()
        {
            return await _context.Users
                .Where(u => u.WorkPlaceAddress != null)
                .Select(b => new User
                {
                    Id = b.Id,
                    FirstName = b.FirstName,
                    LastName = b.LastName,
                    ImageUrl = b.ImageUrl
                })
                .ToListAsync();
        }

    
        public async Task<User> GetBarberByIdAsync(string barberId)
        {
            return await _context.Users.FirstOrDefaultAsync(b => b.Id == barberId);
        }

      
        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations.Include(r => r.Barber).FirstOrDefaultAsync(r => r.Id == id);
        }

      
        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

      
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

