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


    // ------------------------------------
    // REGISTER - GET
    // ------------------------------------

    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }


    // ------------------------------------
    // REGISTER - POST
    // ------------------------------------

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string name,
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "name",
                "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(
                "email",
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "password",
                "Password is required.");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }


        var user = new User
        {
            UserName = email,
            Email = email,
            Name = name
        };


        var result = await _userManager.CreateAsync(
            user,
            password
        );


        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(
                user,
                "User"
            );

            return RedirectToAction("Login");
        }


        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(
                "",
                error.Description
            );
        }


        return View();
    }


    // ------------------------------------
    // LOGIN - GET
    // ------------------------------------

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }


    // ------------------------------------
    // LOGIN - POST
    // ------------------------------------

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);


        if (user == null)
        {
            ModelState.AddModelError(
                "",
                "Invalid email or password."
            );

            return View();
        }


        var result = await _signInManager.PasswordSignInAsync(
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


    // ------------------------------------
    // LOGOUT
    // ------------------------------------

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }


    // ------------------------------------
    // ACCESS DENIED
    // ------------------------------------

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}


}
