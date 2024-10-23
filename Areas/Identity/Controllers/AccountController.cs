using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FinanceManagement;
using FinanceManagement.Models;

using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using FinanceManagement.Models;
using Online_Management.ViewModels;
using OnlineManagement.ViewModels;

namespace Reconciliation.Areas.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(UserManager<ApplicationUser> userManager,
                    SignInManager<ApplicationUser> signInManager,
                    RoleManager<IdentityRole> roleManager,
                    ILogger<AccountController> logger,
                   
                    IDataProtectionProvider dataProtectionProvider,
                    ApplicationDbContext dbContext,
                    IHttpContextAccessor httpContextAccessor
            )
        
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _dataProtector = dataProtectionProvider.CreateProtector("YourPurpose");
            _dbContext = dbContext;

        }

        #region Access Denied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    NameOfUser = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.MobileNumber,
                    CompanyName = model.CompanyName
                    
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Assign "Admin" role to the user
                    await _userManager.AddToRoleAsync(user, "Admin");

                    // Sign in the user after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Login", "Account", new { Area = "Identity" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Login(LoginVM loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                bool isPwdSuccess = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

                if (isPwdSuccess)
                {
                    var result1 = await _signInManager.PasswordSignInAsync(loginViewModel.Email,
                loginViewModel.Password,
                loginViewModel.RememberMe, false);
                    return RedirectToAction("Dashboard", "Home", new { Area = "Admin" });

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials.");
                    return View(loginViewModel);
                }

            }
            return View(loginViewModel);
        }

        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            TempData["success"] = TempData["success"];
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { Area = "Identity" });
        }
        #endregion

        public IActionResult Encrypt(string sensitiveData)
        {
            // Encrypt the sensitive data
            string encryptedData = _dataProtector.Protect(sensitiveData);

            // Return the encrypted data (you might want to return it in a JSON format)
            return Content(encryptedData);
        }

        // Action method to decrypt sensitive data
        public IActionResult Decrypt(string encryptedData)
        {
            // Decrypt the encrypted data
            string decryptedData = _dataProtector.Unprotect(encryptedData);

            // Return the decrypted data (you might want to return it in a JSON format)
            return Content(decryptedData);
        }

        // Action method to set a value in session
        public IActionResult SetSessionValue(string key, string value)
        {
            // Store the value in session
            _httpContextAccessor.HttpContext.Session.SetString(key, value);

            return Ok();
        }

        // Action method to get a value from session
        public IActionResult GetSessionValue(string key)
        {
            // Retrieve the value from session
            var value = _httpContextAccessor.HttpContext.Session.GetString(key);

            if (value != null)
            {
                // Value found, return it
                return Content(value);
            }
            else
            {
                // Value not found
                return NotFound();
            }
        }

        // Action method to remove a value from session
        public IActionResult RemoveSessionValue(string key)
        {
            // Remove the value from session
            _httpContextAccessor.HttpContext.Session.Remove(key);

            return Ok();
        }

        #region ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Generate password reset token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);

                    // Send password reset email with the callback URL
                    using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new NetworkCredential("priyamdhameliya1205@gmail.com", "wzyistwuxqnlohqo");
                        smtpClient.EnableSsl = true;

                        var subject = "Reset your password";
                        var body = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.";
                        var from = "priyamdhameliya1205@gmail.com";
                        var to = user.Email;

                        using (var message = new MailMessage(from, to, subject, body))
                        {
                            message.IsBodyHtml = true;
                            smtpClient.Send(message);
                        }
                    }

                    model.EmailSent = true;
                }
                else
                {
                    // If user is not found, you may choose to display a message indicating that no user with that email exists.
                    ModelState.AddModelError(string.Empty, "No user with that email address exists.");
                }
            }

            return View(model);
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion

        #region ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordVM { UserId = userId, Token = token };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    // Handle case where user is not found
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    // Redirect to password reset confirmation page
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                // Add any errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion



    }
}
