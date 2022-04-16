using FoodApp.Data.Static;
using FoodApp.Data.ViewModel;
using FoodApp.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodApp.Controllers
{
    //account of users
    public class AccountController : Controller
    {
        //inject Usermanger , signinmanger from idenity created to control accoutn
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(IEmailSender emailSender,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;

        }


        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }


        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginVM vm = new LoginVM
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string returnUrl)
        {
            loginVM.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, loginVM.Password)))
            {
                ModelState.AddModelError(string.Empty, "Email is not confirmed yet!");
                return View(loginVM);
            }
            if(user == null)
            {
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(loginVM);

            }

            //var result = await _signInManager.PasswordSignInAsync(loginVM.EmailAddress,
            //                                loginVM.Password, loginVM.RememberMe, false);
            var result = await _signInManager.PasswordSignInAsync(user,
                                          loginVM.Password, loginVM.RememberMe, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Item");
                }
            }

            TempData["Error"] = "Wrong credentials. Please, try again!";
            //ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            return View(loginVM);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", 
                                    new {ReturnUrl = returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginVM loginVM = new LoginVM
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginVM);
            }

            //PasswordHash in AspNetUser table is null because it is ExternalLogin account so no need to store in DB.
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginVM);
            }

            // Get the email claim from external login provider (Google, Facebook etc)
            // or Sign in the user with this external login provider if the user already has a login
            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);

                // If email is not confirmed, display login view with validation error
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email is not confirmed yet!");
                    return View("Login", loginVM);
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //if user has no login account, so add a row in a AspNetUserLogins table in DB.
                if (email != null)
                {
                    if(user == null)
                    {
                        user = new ApplicationUser { UserName = email, Email = email };
                        await _userManager.CreateAsync(user);

                        // After a local user account is created, generate and log the email confirmation link
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                            "email, by clicking on the confirmation link we have emailed you.";
                        return View("ConfirmErrorView");
                    }

                    await _userManager.AddLoginAsync(user, loginInfo);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not recieved from: {loginInfo.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on yummy@gmail.com";

                return View("ConfirmErrorView");
            }
        }


        public IActionResult Register() => View(new RegisterVM());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            //Check if email exist 
             var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);

             if (user != null)
             {
                 TempData["Error"] = "This email address is already in use";
                 return View(registerVM);
             }

             var newUser = new ApplicationUser()
             {
                 FullName = registerVM.FullName,
                 Email = registerVM.EmailAddress,
                 UserName = registerVM.EmailAddress
             };

             var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

             if (newUserResponse.Succeeded)
             {
                 var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                 var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                         new { userId = newUser.Id, token = token }, Request.Scheme);
                 _logger.Log(LogLevel.Warning, confirmationLink);

                 await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                 ViewBag.ErrorTitle = "Registration successful";
                 ViewBag.ErrorMessage = "Before you can login, please confirm your email, " +
                     "by clicking on the confirmation link we have emailed you.";
                 return View("ConfirmErrorView");
             }

            return View("RegisterCompleted");
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Item");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            { 
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("ConfirmErrorView");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

                // If the user is found AND Email is confirmed
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = forgotPasswordVM.Email, token = token }, Request.Scheme);

                    // Log the password reset link
                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(forgotPasswordVM);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(resetPasswordVM);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(resetPasswordVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Item");
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactusAsync(ContactUsVM contactUsVM)
        { 
            var msg = contactUsVM.FullName + " " + contactUsVM.Notes;
            await _emailSender.SendEmailAsync(contactUsVM.EmailAddress, "Contact mail", msg);
            ViewBag.ConfirmMsg = "Message recieved";

            return View();
        }
    }
}