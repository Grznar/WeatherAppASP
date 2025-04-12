using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;
using WeatherApp.Application.Common.Dto;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RecaptchaService _recaptchaService;
        public AuthController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, RecaptchaService recaptchaService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _recaptchaService = recaptchaService;
            
        }
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            LoginVM loginVM = new()
            {

            };
            return View(loginVM);
        }
        public async Task<IActionResult> Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
           

            RegisterVM registerVM = new()
            {
               
            };
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var recaptchaToken = Request.Form["g-recaptcha-response"].ToString();
                Console.WriteLine("Recaptcha token: " + recaptchaToken);
                var isCaptchaValid = await _recaptchaService.VerifyResponseAsync(recaptchaToken);
                Console.WriteLine(isCaptchaValid);
                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", "ReCaptcha Authent Failed ");
                    return View(registerVM);
                }
                ApplicationUser user = new()
                {
                    UserName = registerVM.Email,
                    Email = registerVM.Email,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = true,
                    


                };


                var result_ = _userManager.CreateAsync(user, registerVM.Password).GetAwaiter().GetResult();

                if (result_.Succeeded)
                {
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");



                }

                foreach (var error in result_.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
            }
            return View(registerVM);


        }
        public async Task<IActionResult> Logout()
        {
           await  _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, true, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(loginVM.Email);
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    ModelState.AddModelError("", "Invalid Login Attempt");
                }
            }
            return View(loginVM);
        }
    }
}
