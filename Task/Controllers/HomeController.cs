using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Task.Entities;
using Task.Models;
using Task.Options;
using MimeKit.Text;

namespace Task.Controllers
{
    [AutoValidateAntiforgeryToken]

    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private EmailServerOptions _emailServerOptions;

        public HomeController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptionsMonitor<EmailServerOptions> emailServerOptions,
            IWebHostEnvironment env)
        {
            _env = env;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailServerOptions = emailServerOptions.CurrentValue;
            emailServerOptions.OnChange((e) =>
            {
                _emailServerOptions = e;
            });
        }

    

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new UserCreateModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Email = model.Email,
                    Gender = model.Gender,
                    UserName = model.Username,
                };



                var identityResult = await _userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    var memberRole = await _roleManager.FindByNameAsync("Member");
                    if (memberRole == null)
                    {
                        await _roleManager.CreateAsync(new()
                        {
                            Name = "Member",
                            CreateTime = DateTime.Now
                        });

                    }

                    await _userManager.AddToRoleAsync(user, "Member");

                    return RedirectToAction("Index");
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        public IActionResult SignIn(string returnUrl)
        {

            return View(new UserSignInModel { ReturnUrl = returnUrl });

        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);

                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }



                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminPanel");
                    }

                    else
                    {
                        return RedirectToAction("Panel");
                    }
                }

                else if (signInResult.IsLockedOut)
                {
                    var lockOutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    var now = DateTime.UtcNow;

                    ModelState.AddModelError("", $"Your account has been {(lockOutEnd.Value.UtcDateTime - DateTime.UtcNow).Minutes} minute suspended");
                }
                else
                {
                    var message = string.Empty;


                    if (user != null)
                    {
                        var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                        message = $"{(_userManager.Options.Lockout.MaxFailedAccessAttempts - failedCount)}  If you enter it again, your account will be temporarily locked";
                    }
                    else
                    {
                        message = "Username or Password is incorrect";
                    }

                    ModelState.AddModelError("", message);

                }
            }



            return View(model);
        }


        public IActionResult GetUserInfo()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            User.IsInRole("Member");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public IActionResult Panel()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public IActionResult MemberPage()
        {
            return View();
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index", "User");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordVM);
            }

            AppUser user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Test", "test.testov.2004@bk.ru"));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = "Reset Password";

            string emailBody = string.Empty;

            using (StreamReader streamReader = new StreamReader(Path.Combine(_env.WebRootPath, "templates", "mail.html")))
            {
                emailBody = streamReader.ReadToEnd();
            }

            string forgotPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            string url = Url.Action("changepassword", "home", new { Id = user.Id, token = forgotPasswordToken }, Request.Scheme);

            emailBody = emailBody.Replace("{{url}}", $"{url}");

            message.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.Connect("smtp.bk.ru", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("test.testov.2004@bk.ru", "xAhSKKbBRqCKCJgtpyw4");
            smtp.Send(message);
            smtp.Disconnect(true);

            return RedirectToAction("SignIn");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                ModelState.AddModelError("", $"User with id {id} was not found");
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, changePasswordVM.Token, changePasswordVM.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Could not change user's password");
            }

            return RedirectToAction("SignIn");
        }
    }
}
