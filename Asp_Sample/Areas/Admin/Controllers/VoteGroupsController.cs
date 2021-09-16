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
    [Authorize(Roles = "Admin" )]
    [Area("Admin")]
    public class VoteGroupsController : Controller
    {
        private readonly AppDbContext _context;

        public VoteGroupsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/VoteGroups
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.VoteGroups.Include(v => v.Group).Include(v => v.Vote);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/VoteGroups/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voteGroup = await _context.VoteGroups
                .Include(v => v.Group)
                .Include(v => v.Vote)
                .FirstOrDefaultAsync(m => m.VoteGroupId == id);
            if (voteGroup == null)
            {
                return NotFound();
            }

            return View(voteGroup);
        }

        // GET: Admin/VoteGroups/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle");
            ViewData["VoteId"] = new SelectList(_context.Votes, "VoteId", "VoteName");
            return View();
        }

        // POST: Admin/VoteGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoteGroupId,GroupId,VoteId")] VoteGroup voteGroup)
        {
            if (ModelState.IsValid)
            {
                voteGroup.VoteGroupId = Guid.NewGuid();
                _context.Add(voteGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle", voteGroup.GroupId);
            ViewData["VoteId"] = new SelectList(_context.Votes, "VoteId", "VoteName", voteGroup.VoteId);
            return View(voteGroup);
        }

        // GET: Admin/VoteGroups/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voteGroup = await _context.VoteGroups.FindAsync(id);
            if (voteGroup == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle", voteGroup.GroupId);
            ViewData["VoteId"] = new SelectList(_context.Votes, "VoteId", "VoteName", voteGroup.VoteId);
            return View(voteGroup);
        }

        // POST: Admin/VoteGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("VoteGroupId,GroupId,VoteId")] VoteGroup voteGroup)
        {
            if (id != voteGroup.VoteGroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voteGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await VoteGroupExists(voteGroup.VoteGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "GroupTitle", voteGroup.GroupId);
            ViewData["VoteId"] = new SelectList(_context.Votes, "VoteId", "VoteName", voteGroup.VoteId);
            return View(voteGroup);
        }

        // GET: Admin/VoteGroups/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voteGroup = await _context.VoteGroups
                .Include(v => v.Group)
                .Include(v => v.Vote)
                .FirstOrDefaultAsync(m => m.VoteGroupId == id);
            if (voteGroup == null)
            {
                return NotFound();
            }

            return View(voteGroup);
        }

        // POST: Admin/VoteGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var voteGroup = await _context.VoteGroups.FindAsync(id);
            _context.VoteGroups.Remove(voteGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> VoteGroupExists(Guid id)
        {
            return await  _context.VoteGroups.AnyAsync(e => e.VoteGroupId == id);
        }
    }
}
