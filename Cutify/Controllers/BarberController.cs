using System;
using System.Security.Claims;
using Cutify.DATA;
using Cutify.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Controllers
{

    [Authorize]
    public class BarberController:Controller
	{
        private readonly IReservationRepository _reservationRepository;

        public BarberController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IActionResult> Reservations(DateTime? date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservations = await _reservationRepository.GetReservationsByBarberIdAsync(userId, date);

            return View(reservations);
        }

    }
}

