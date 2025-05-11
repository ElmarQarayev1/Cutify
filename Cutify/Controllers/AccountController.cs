using System;
using System.Security.Claims;
using Cutify.DATA;
using Cutify.Helpers;
using Cutify.Models;
using Cutify.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Controllers
{
	public class AccountController:Controller
	{
		private readonly AppDbContext_ appDbContext_;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
      AppDbContext_ appDbContext__,
      UserManager<User> userManager,
      SignInManager<User> signInManager,
            EmailService emailService, ILogger<AccountController> _logger_)
        {
            appDbContext_ = appDbContext__;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = _logger_;
        }
        [AllowAnonymous]
        public IActionResult Login()
		{
			return View();
		}

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View(loginVM);
                }

                var user = await _userManager.FindByNameAsync(loginVM.Username);
                if (user == null)
                {
                    ModelState.AddModelError("", "Belə bir istifadəçi tapılmadı.");
                    return View(loginVM);
                }

                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return RedirectToAction("Reservations", "Barber");
                }


                ModelState.AddModelError("", "Daxilolma uğursuz oldu. İstifadəçi adı və ya şifrə səhvdir.");
                return View(loginVM);
            }
            catch(Exception ex)
            {

                _logger.LogError(ex, "Login failed for user {Username}", loginVM.Username);

                var exceptionLog = new ExceptionLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    CreatedAt = DateTime.UtcNow,
                    Source = "Login"
                };

                await appDbContext_.ExceptionLogs.AddAsync(exceptionLog);
                await appDbContext_.SaveChangesAsync();

                return View("Error");
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


                HttpContext.Session.Clear();

                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");

                var exceptionLog = new ExceptionLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    CreatedAt = DateTime.UtcNow,
                    Source = "Logout"
                };

                await appDbContext_.ExceptionLogs.AddAsync(exceptionLog);
                await appDbContext_.SaveChangesAsync();

                return View("Error");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            try
            {

            
            if (!ModelState.IsValid) return View(registerVM);


                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Bu email ilə artıq bir istifadəçi mövcuddur.");
                    return View(registerVM);
                }

                var user = new User
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = (registerVM.FirstName + " " + registerVM.LastName).Replace(" ", ""),
                WorkPlaceAddress = registerVM.WorkPlaceAddress
            };
            if (registerVM.ImageFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(registerVM.ImageFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await registerVM.ImageFile.CopyToAsync(stream);
                }

                user.ImageUrl = "/uploads/" + fileName;
            }

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register failed");

                var exceptionLog = new ExceptionLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    CreatedAt = DateTime.UtcNow,
                    Source = "Logout"
                };

                await appDbContext_.ExceptionLogs.AddAsync(exceptionLog);
                await appDbContext_.SaveChangesAsync();

                return View("Error");

            }
        }


        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM vm)
        {
            try
            {

           
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu email ilə istifadəçi tapılmadı.");
                return View(vm);
            }

           
            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

         
            HttpContext.Session.SetString("ResetCode", code);
            HttpContext.Session.SetString("ResetEmail", vm.Email);

    
            var subject = "Şifrə sıfırlama təsdiq kodu";
                var body = $@"
    <div style='font-family:Arial, sans-serif; padding:20px; background-color:#f9f9f9; color:#333;'>
        <div style='max-width:600px; margin:0 auto; background-color:#ffffff; padding:30px; border-radius:8px; box-shadow:0 2px 5px rgba(0,0,0,0.1);'>
            <h2 style='color:#007bff;'>Şifrə Sıfırlama İstəyi</h2>
            <p>Salam <strong>{user.FullName}</strong>,</p>
            <p>Şifrənizi sıfırlamaq üçün aşağıdakı təsdiq kodunu istifadə edin:</p>
            <div style='font-size:24px; font-weight:bold; color:#d9534f; margin:20px 0;'>{code}</div>
            <p>Bu kod yalnız bir dəfə istifadə edilə bilər və müəyyən müddət ərzində etibarlıdır.</p>
            <p>Əgər bu istəyi siz etməmisinizsə, bu emaili nəzərə almayın.</p>
            <br/>
            <p>Hörmətlə,</p>
            <p><em>CUTIFY</em></p>
        </div>
    </div>";
                _emailService.Send(user.Email, subject, body);

            return RedirectToAction("VerifyCode");

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "ForgetPassword failed");

                var exceptionLog = new ExceptionLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    CreatedAt = DateTime.UtcNow,
                    Source = "Logout"
                };
                await appDbContext_.ExceptionLogs.AddAsync(exceptionLog);
                await appDbContext_.SaveChangesAsync();

                return View("Error");

            }
        }

        [AllowAnonymous]
        public IActionResult VerifyCode()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (email == null)
            {
               
                return RedirectToAction("Error");
            }

            var model = new VerifyCodeViewModel { Email = email };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCode(string code)
        {
            var savedCode = HttpContext.Session.GetString("ResetCode");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (savedCode == code)
            {
                return RedirectToAction("ResetPassword", new { email = email });
            }

            ModelState.AddModelError("", "Kod yanlışdır");
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordVM
            {
                Email = email
            };
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (vm.NewPassword != vm.ConfirmNewPassword)
            {
                ModelState.AddModelError("", "Yeni şifrə və təsdiq şifrəsi uyğun gəlmir.");
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "İstifadəçi tapılmadı.");
                return View(vm);
            }

          
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                foreach (var error in removePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }

           
            var addPasswordResult = await _userManager.AddPasswordAsync(user, vm.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendCode()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (email == null)
            {
                return RedirectToAction("Error");
            }

           
            var newCode = GenerateNewVerificationCode();

           
            HttpContext.Session.SetString("ResetCode", newCode);

         
            await SendVerificationCode(email, newCode);

            return Json(new { success = true, message = "Yeni kod gönderildi!" });
        }

        private string GenerateNewVerificationCode()
        {
            
            var code = new Random().Next(1000, 9999).ToString();
            return code;
        }

        public async Task SendVerificationCode(string email, string newCode)
        {
           
            HttpContext.Session.SetString("ResetCode", newCode);
            HttpContext.Session.SetString("ResetEmail", email);

            var subject = "Şifrə sıfırlama təsdiq kodu";
            var body = $"<p>Şifrənizi yeniden sıfırlamaq üçün bu kodu istifadə edin: <strong>{newCode}</strong></p>";

           
            _emailService.Send(email, subject, body);
        }


    }
}

