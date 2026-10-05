


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockPilot.Models;

namespace StockPilot.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // REGISTER PAGE
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        // REGISTER USER
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string name,
            string email,
            string password,
            string confirmPassword)
        {
            // Check name
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(
                    "",
                    "Name is required."
                );
            }


            // Check email
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "",
                    "Email is required."
                );
            }


            // Check password
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "",
                    "Password is required."
                );
            }


            // Check confirm password
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError(
                    "",
                    "Confirm password is required."
                );
            }


            // Check both passwords
            if (!string.IsNullOrWhiteSpace(password) &&
                !string.IsNullOrWhiteSpace(confirmPassword))
            {
                if (password != confirmPassword)
                {
                    ModelState.AddModelError(
                        "",
                        "Password and Confirm Password do not match."
                    );
                }
            }


            // If there is an error
            if (!ModelState.IsValid)
            {
                return View();
            }


            // Check whether email already exists
            var oldUser =
                await _userManager.FindByEmailAsync(email);

            if (oldUser != null)
            {
                ModelState.AddModelError(
                    "",
                    "This email is already registered."
                );

                return View();
            }


            // Create user
            var user = new User();

            user.UserName = email;
            user.Email = email;
            user.Name = name;


            var result =
                await _userManager.CreateAsync(
                    user,
                    password
                );


            // User created
            if (result.Succeeded)
            {
                // Give User role
                var roleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        "User"
                    );


                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description
                        );
                    }

                    return View();
                }


                // Go to login page
                return RedirectToAction("Login");
            }


            // Show registration errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description
                );
            }


            return View();
        }


        // LOGIN PAGE
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        // LOGIN USER
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password)
        {
            var user =
                await _userManager.FindByEmailAsync(email);


            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password."
                );

                return View();
            }


            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    password,
                    false,
                    false
                );


            if (result.Succeeded)
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard"
                );
            }


            ModelState.AddModelError(
                "",
                "Invalid email or password."
            );

            return View();
        }


        // LOGOUT
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }


        // ACCESS DENIED
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
