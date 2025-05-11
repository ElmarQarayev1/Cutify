using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cutify.DATA;
using Cutify.Models;
using Cutify.Models.ViewModels;
using Cutify.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cutify.Controllers
{

    [AllowAnonymous]
    public class ReservationController : Controller
    {
        private readonly ICreateReservations _reservationRepository;

        public ReservationController(ICreateReservations reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IActionResult> SelectBarber()
        {
            var barbers = await _reservationRepository.GetAllBarbersWithWorkplaceAsync();
            return View(barbers);
        }

        public async Task<IActionResult> PersonalInfo(string barberId)
        {
            var barber = await _reservationRepository.GetBarberByIdAsync(barberId);
            if (barber == null) return RedirectToAction("SelectBarber");

            var model = new ReservationViewModel
            {
                BarberId = barberId,
                ReservationDate = DateTime.Now.Date.AddDays(1).AddHours(10),
                CustomerFullName = Request.Cookies["CustomerFullName"],
                CustomerPhone = Request.Cookies["CustomerPhone"]
            };

            ViewBag.BarberFullName = barber.FullName;
            ViewBag.BarberId = barberId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PersonalInfo(ReservationViewModel reservationViewModel)
        {
            if (ModelState.IsValid)
            {
                if (reservationViewModel.CustomerPhone.Length < 10 || reservationViewModel.CustomerPhone.Length > 15)
                {
                    ModelState.AddModelError("CustomerPhone", "Telefon nömrəsi 10-15 simvol arasında olmalıdır.");
                    return View(reservationViewModel);
                }

                if (reservationViewModel.ReservationDate.Date < DateTime.Today)
                {
                    ModelState.AddModelError("ReservationDate", "Keçmiş tarix seçilə bilməz.");
                    var barberb = await _reservationRepository.GetBarberByIdAsync(reservationViewModel.BarberId);
                    ViewBag.BarberFullName = barberb?.FullName;
                    return View(reservationViewModel);
                }
                var reservation = new Reservation
                {
                    CustomerFullName = reservationViewModel.CustomerFullName,
                    CustomerPhone = reservationViewModel.CustomerPhone,
                    BarberId = reservationViewModel.BarberId,
                    ReservationDate = reservationViewModel.ReservationDate
                };
                if (!string.IsNullOrEmpty(reservationViewModel.TimeInput))
                {
                    var timeParts = reservationViewModel.TimeInput.Split(':');
                    if (timeParts.Length == 2)
                    {
                        var hours = int.Parse(timeParts[0]);
                        var minutes = int.Parse(timeParts[1]);
                        reservation.ReservationDate = reservation.ReservationDate.Date.AddHours(hours).AddMinutes(minutes);
                    }
                }

                await _reservationRepository.AddReservationAsync(reservation);
                await _reservationRepository.SaveChangesAsync();

                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(365)
                };
                Response.Cookies.Append("CustomerFullName", reservationViewModel.CustomerFullName, options);
                Response.Cookies.Append("CustomerPhone", reservationViewModel.CustomerPhone, options);

                return RedirectToAction("Success");
            }

            var barber = await _reservationRepository.GetBarberByIdAsync(reservationViewModel.BarberId);
            if (barber == null) return RedirectToAction("SelectBarber");

            ViewBag.BarberFullName = barber.FullName;
            return View(reservationViewModel);
        }

        public IActionResult Success()
        {
            return View();
        }


        public async Task<IActionResult> Confirmation(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null) return RedirectToAction("Index");

            return View(reservation);
        }
    }




}