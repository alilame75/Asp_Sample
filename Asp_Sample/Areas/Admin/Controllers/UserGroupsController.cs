using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLayer.Context;
using DataLayer.Entity.Group;
using Microsoft.AspNetCore.Authorization;

namespace Asp_Sample.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UserGroupsController : Controller
    {
        private readonly AppDbContext _context;

        public UserGroupsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserGroups
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.UserGroups.Include(u => u.Group).Include(u => u.User);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> GroupUsers(Guid groupId)
        {
            var appDbContext = _context.UserGroups
                .Include(u => u.Group)
                .Include(u => u.User)
                .Where(u => u.GroupId == groupId);
            var name = await _context.Groups.Where(c => c.GroupId == groupId).Select(c => c.GroupTitle).FirstOrDefaultAsync();
            if(name == null)
            {
                name = "بدون نام";
            }
            ViewData["GroupId"] = groupId;
            ViewData["Name"] = name;
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/UserGroups/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups
                .Include(u => u.Group)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserGroupId == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // GET: Admin/UserGroups/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        public IActionResult AddUser(Guid groupId)
        {
            ViewData["GroupId"] = new SelectList(_context.Groups.Where(g => g.GroupId == groupId), "GroupId", "GroupTitle");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View("Create");
        }

        // POST: Admin/UserGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserGroupId,GroupId,UserId")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                userGroup.UserGroupId = Guid.NewGuid();
                var flag = await _context.UserGroups
                    .Where(c => c.UserId == userGroup.UserId && c.GroupId == userGroup.GroupId).AnyAsync();
                if(!flag)
                {
                    await _context.AddAsync(userGroup);
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index), "Groups");
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle", userGroup.GroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userGroup.UserId);
            return View(userGroup);
        }

        // GET: Admin/UserGroups/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle", userGroup.GroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userGroup.UserId);
            return View(userGroup);
        }

        // POST: Admin/UserGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserGroupId,GroupId,UserId")] UserGroup userGroup)
        {
            if (id != userGroup.UserGroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await UserGroupExists(userGroup.UserGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), "Groups");
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle", userGroup.GroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userGroup.UserId);
            return View(userGroup);
        }

        // GET: Admin/UserGroups/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups
                .Include(u => u.Group)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserGroupId == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // POST: Admin/UserGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);
            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Groups");
        }

        private async Task<bool> UserGroupExists(Guid id)
        {
            return await  _context.UserGroups.AnyAsync(e => e.UserGroupId == id);
        }
    }
}
