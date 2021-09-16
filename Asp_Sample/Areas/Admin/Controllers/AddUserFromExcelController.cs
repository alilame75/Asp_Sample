using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Utilities;
using DataLayer.Entity.User;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataLayer.Entity.Group;

namespace Asp_Sample.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] //   /Admin/AddUserFromExcel
    public class AddUserFromExcelController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;


        public AddUserFromExcelController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var excelFile = ExcelAdaptor.GetExcel();


            var newUsers = new List<AppUser>();
            var existsUsers = new List<AppUser>();

            foreach (var item in excelFile)
            {
                var userFlag = await _dbContext.Users
                    .Where(user => user.Email.ToLower() == item.Email.ToLower() || user.UserName.ToLower() == item.UserName.ToLower())
                    .FirstOrDefaultAsync();

                if(userFlag == null)
                {
                    item.Id = Guid.NewGuid().ToString();
                    newUsers.Add(new AppUser()
                    {
                        Id = item.Id,
                        UserName = item.UserName,
                        Email = item.Email,
                        RegistrationTime = DateTime.Now,
                        Active = true,
                        PasswordHash = item.Password,
                    });
                    
                }
                else
                {
                    existsUsers.Add(userFlag);
                    item.Id = userFlag.Id;
                }
            }

            foreach(var item in newUsers)
            {
                try
                {
                    var password = item.PasswordHash;
                    item.PasswordHash = null;
                    var result = await _userManager.CreateAsync(item, password);
                }
                catch
                {
                    return Content($"تمامی کاربران بالای کاربر {item.UserName} اضافه شدند");
                }
            }

            var groupsTitle = excelFile.Select(s => s.Group).Distinct().ToList();
            var allGroups = new List<Group>();
            foreach(var item in groupsTitle)
            {
                var group = await _dbContext.Groups.Where(c => c.GroupTitle == item).FirstOrDefaultAsync();
                if(group == null)
                {
                    return Content($"گروه {group} وجود ندارد");
                }
                allGroups.Add(group);
            }

            foreach(var item in allGroups)
            {
                var usersIdsInGroups = excelFile.Where(c => c.Group == item.GroupTitle)
                    .Select(u => u.Id).ToList();

                var exsitUser = await _dbContext.UserGroups.Where(g => g.GroupId == item.GroupId).Select(s => s.UserId).ToListAsync();

                var usergroups = new List<UserGroup>();
                foreach(var user in usersIdsInGroups)
                {
                    if(!existsUsers.Any(c => c.Id == user))
                    {
                        usergroups.Add(new UserGroup()
                        {
                            UserGroupId = Guid.NewGuid(),
                            GroupId = item.GroupId,
                            UserId = user,
                        });
                    }
                }
                await _dbContext.UserGroups.AddRangeAsync(usergroups);
                await _dbContext.SaveChangesAsync();
            }

            return Ok("تامام");

        }



        [HttpGet]
        public async Task<IActionResult> Fix()
        {
            var excelFile = ExcelAdaptor.GetExcel();
            var existUsers = new List<AppUser>();


            foreach(var item in excelFile)
            {
                var userInDb = await _dbContext.Users
                    .Where(c => c.Email.ToLower() == item.Email.ToLower()).FirstOrDefaultAsync();
                if(userInDb == null)
                {
                    continue;
                }

                await _userManager.SetUserNameAsync(userInDb, item.UserName);
                await _userManager.RemovePasswordAsync(userInDb);
                await _userManager.AddPasswordAsync(userInDb, item.Password);
            }
            return Ok("تامام");
        }
    }
}
