using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task.Context;
using Task.Entities;
using Task.Models;

namespace Task.Controllers
{

    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, dbContext context, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            //    var query = _userManager.Users;
            //    var users = _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new 
            //    { 
            //        user,
            //        userRole
            //    }).Join(_context.Roles, two => two.userRole.RoleId, role => role.Id, (two, role) => new {two.user,two.userRole,role}).Where(x=>x.role.Name!="Admin").Select(x=> new AppUser
            //    {
            //        Id = x.user.Id,
            //        AccessFailedCount = x.user.AccessFailedCount,
            //        ConcurrencyStamp = x.user.ConcurrencyStamp,
            //        Email = x.user.Email,
            //        EmailConfirmed = x.user.EmailConfirmed,
            //        Gender = x.user.Gender,
            //        ImagePath = x.user.ImagePath,
            //        LockoutEnabled = x.user.LockoutEnabled,
            //        LockoutEnd = x.user.LockoutEnd,
            //        NormalizedEmail = x.user.NormalizedEmail,
            //        NormalizedUserName = x.user.NormalizedUserName,
            //        PasswordHash = x.user.PasswordHash,
            //        PhoneNumber = x.user.PhoneNumber,
            //        UserName = x.user.UserName,
            //    }).ToList();

            var users = await _userManager.GetUsersInRoleAsync("Member");

            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserAdminCreateModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserAdminCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Username,
                    Gender = model.Gender
                };

                var result = await _userManager.CreateAsync(user, model.Username + "123");

                if (result.Succeeded)
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

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }

            return View(model);
        }

        public async Task<IActionResult> UserEdit(string id)
        {
            var userEdit = await _userManager.FindByIdAsync(id);
            if (userEdit == null)
            {
                return NotFound();
            }
            UserEditVM model = new()
            {
                Email = userEdit.Email,
                UserName = userEdit.UserName,
                Gender = userEdit.Gender,
               

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(string id, UserEditVM model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (!ModelState.IsValid)
            {
                return View();
            }
            user.Email = model.Email;
            user.Gender = model.Gender;
            user.UserName = model.UserName;

            var existingRoles = await _userManager.GetRolesAsync(user); 
            await _userManager.RemoveFromRolesAsync(user, existingRoles);

            var selectedRoles = model.SelectedRoles;
            await _userManager.AddToRolesAsync(user, selectedRoles);

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
    }
}
