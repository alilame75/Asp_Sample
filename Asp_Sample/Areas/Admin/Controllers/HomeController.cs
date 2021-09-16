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
using Services.DTOs.Vote;
using Services.Interfaces;
using WebFramework.Message;
using Asp_Sample.Areas.Admin.Models;

namespace Asp_Sample.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IVoteService _voteService;
        private readonly AppDbContext _dbContext;

        public HomeController(IVoteService voteService, AppDbContext dbContext)
        {
            _voteService = voteService;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Votes"] = await  _voteService.GetAllVote();
            ViewData["Messages"] = TempData["Messages"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddVote()
        {
            var groups = await _dbContext.Groups.ToListAsync();
            List<GroupSelectForVote> groupSelectForVotes = new List<GroupSelectForVote>();
            foreach (var @group in groups)
            {
                groupSelectForVotes.Add(new GroupSelectForVote()
                {
                    GroupId = @group.GroupId,
                    Selected = false,
                    GroupTitle = @group.GroupTitle,
                });
            }

            ViewData["Groups"] = groupSelectForVotes;

            return  View();
        }


        [HttpPost]
        public async Task<IActionResult> AddVote(VoteDto model)
        {
            await _voteService.AddVoteAsync(model);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteVote(Guid id)
        {
            var userVoteAnswersDatas = await _dbContext.UserVoteAnswers.Where(e => e.VoteId == id).ToListAsync();
            _dbContext.UserVoteAnswers.RemoveRange(userVoteAnswersDatas);
            var voteAnswersDatas = await _dbContext.VoteAnswers.Where(e => e.VoteId == id).ToListAsync();
            _dbContext.VoteAnswers.RemoveRange(voteAnswersDatas);
            var votesDatas = await _dbContext.Votes.FindAsync(id);
            _dbContext.Votes.Remove(votesDatas);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

      
        public IActionResult Vote(Guid id)
        {

            return RedirectToPage("Account");
        }

        public async Task<IActionResult> ActiveDeActive(Guid id)
        {
            TempData["Messages"] = new List<MessagesForView>()
            {
                new MessagesForView()
                {
                    Message = "تست سبز",MessageStatus = MessageStatus.success,
                }
            };

            var res = await _dbContext.Votes.FindAsync(id);
            res.Active = !res.Active;

            _dbContext.Update(res);
            await _dbContext.SaveChangesAsync();
            

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ShowResponse(Guid id)
        {
            ViewData["Response"] = await  _voteService.GetVoteCount(id);

            ViewData["UserVoted"] = await _dbContext.UserVoteAnswers
                .Where(a => a.VoteId == id)
                .Include(u => u.User)
                .GroupBy(c => c.User.UserName)
                .Select(ua => new UserVoted()
                {
                    Username = ua.Key,
                    Count = ua.Count()
                }).ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditVoteTime(Guid id)
        {
            var model = await _dbContext.Votes.Where(c => c.VoteId == id)
                .Select(c => new VoteTimeDto() { 
                    VoteId = c.VoteId,
                    VoteTime = c.FinishTime,
                }).FirstOrDefaultAsync();
            return View(model);
        }

        // حل است ؟

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoteTime(VoteTimeDto dto)
        {
            var vote = await _dbContext.Votes.FindAsync(dto.VoteId);
            if(vote.FinishTime != dto.VoteTime)
            {
                vote.FinishTime = dto.VoteTime;
                _dbContext.Entry(vote).Property(c => c.FinishTime).IsModified = true;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public class UserVoted
        {
            public string Username { get; set; }
            public int Count { get; set; }
        }


    }
}