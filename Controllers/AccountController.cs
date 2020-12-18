using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookSharing.Utilities;
using BookSharing.ViewModels;
using BookSharing.Data;

namespace BookSharing.Controllers
{

    [Authorize(Roles = "admin")]
    public class AccountController : Controller
    {

        private readonly BookSharingDbContext _context;

        private readonly UserManager<IdentityUser> _UserManager;

        private readonly SignInManager<IdentityUser> _SignInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(BookSharingDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _UserManager = userManager;
            _SignInManager = signInManager;
            _roleManager = roleManager;

        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]

        public IActionResult ListAllUsers()
        {
            var model = _UserManager.Users;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string Id)
        {

            var user = await _UserManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _UserManager.GetRolesAsync(user);

            ViewBag.UserRoles = roles;

            return View(user);

        }

        [HttpGet]

        public async Task<IActionResult> Remove(string Id)
        {
            var user = await _UserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("ListAllUsers", "Account");
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RemoveConfirmed(string Id)
        {
            var user = await _UserManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["RemoveSuccess"] = user.UserName + " removed successfully!";
                return RedirectToAction("ListAllUsers", "Account");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            return View(user);
        }

        [HttpGet]

        public async Task<IActionResult> ManageUserRoles(string Id)
        {

            var user = await _UserManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            var listModel = new List<ManageUserRolesViewModel>();
            foreach (var item in _roleManager.Roles.ToList())
            {
                var model = new ManageUserRolesViewModel
                {
                    RoleId = item.Id,
                    RoleName = item.Name
                };

                model.IsSelected = await _UserManager.IsInRoleAsync(user, item.Name) ? true : false;

                listModel.Add(model);
            }

            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;

            return View(listModel);
        }

        [HttpPost]

        public async Task<IActionResult> ManageUserRoles(string Id, List<ManageUserRolesViewModel> model)
        {
            var user = await _UserManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            foreach (var item in model)
            {
                var role = await _roleManager.FindByIdAsync(item.RoleId);

                if (item.IsSelected && !await _UserManager.IsInRoleAsync(user, role.Name))
                {
                    await _UserManager.AddToRoleAsync(user, role.Name);
                }
                else if (!item.IsSelected && await _UserManager.IsInRoleAsync(user, role.Name))
                {
                    await _UserManager.RemoveFromRoleAsync(user, role.Name);
                }

            }

            return RedirectToAction("ListAllUsers", "Account");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email,Password,RememberMe")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _UserManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && await _UserManager.CheckPasswordAsync(user, model.Password))
                {

                    ModelState.AddModelError(string.Empty, "Your email is not confirmed");
                    return View(model);
                }

                var result = await _SignInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

            }

            ModelState.AddModelError(" ", "Login Failed");

            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([Bind("Email,Password,ConfirmPassword")] RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    user = await _UserManager.FindByEmailAsync(user.Email);
                    var token = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.ActionLink("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    var email = new EmailComposer
                    {
                        emailAddressSender = "29187@aeamatolusitano.edu.pt",
                        emailAddressRecipient = user.Email,
                        smtpServerAddress = "smtp.gmail.com",
                        port = 587,
                        password =                                                                                                                                 "biciado123LOLvv1",
                        subject = "Email Confirmation",
                        body = "Please confirm your email in order to log on to our RawIndetity web application. Click on the following link..." + confirmationLink

                    };

                    email.SendEmail();
                    ViewBag.Message = "We sent you confirmation request for your email. Please check your mailbox";

                    return View("ConfirmEmail");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _UserManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                ViewBag.Message = "Account does not exist!";
                return View();
            }

            var result = await _UserManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                ViewBag.Message = "Your email was successfully confirmed";
                return View();
            }

            ViewBag.Message = "Your email confirmation failed!";
            return View();

        }


        [AllowAnonymous]
        [HttpPost]

        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

}