using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Entity.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp_Sample.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        #region IOC

        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UsersController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        #endregion



        public  IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/admin/api/GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var res = await _userManager.Users.Select(u => new UserForViewAdmin()
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                RegistrationTime = u.RegistrationTime,
                EmailConfirm = u.EmailConfirmed,
                Active = u.Active,

            }).ToListAsync();

            return Json(res);
        }

        public async Task<IActionResult> ActiveDeActiveUser(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            user.Active = !user.Active;
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);

            return RedirectToAction("Index");
        }

        public class UserForViewAdmin
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public DateTime RegistrationTime { get; set; }
            public bool EmailConfirm { get; set; }
            public bool Active { get; set; }
        }


        
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] UserDeleteDto userDto)
        {

            var id = userDto.Id;

            if (id == null)
            {
                return NotFound();
            }

            var @user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(@user);

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: Admin/Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @user = await _context.Users.FindAsync(id);
            _context.Users.Remove(@user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GroupExists(string id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }

        public class UserDeleteDto
        {
            public string Id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserPassword([FromBody] UserDeleteDto userDto)
        {
            AppUser user = await  _context.Users.FindAsync(userDto.Id);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token,"Aa@123456789");
            
            return RedirectToAction("Index");
        }

    }
}