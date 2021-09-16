using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Entity.Group;
using DataLayer.Entity.Vote;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.Vote;
using Services.Interfaces;

namespace Services.Services
{
    public class VoteService : IVoteService
    {
        private readonly AppDbContext _context;

        public VoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vote>> GetAllVote(bool activeFilter = false)
        {
            if (activeFilter)
            {
                return await _context.Votes.Where(v => v.Active == true).OrderBy(c => c.Priority).ToListAsync();
            }
            return await _context.Votes.OrderBy(c => c.Priority).ToListAsync();
        }

        public async Task<List<Vote>> GetAllVote(string userId, bool activeFilter = false)
        {
            if (activeFilter)
            {
                return await _context.Users.Where(c => c.Id == userId)
                    .SelectMany(c => c.UserGroups).Select(c => c.Group)
                    .SelectMany(c => c.VoteGroups).Select(v => v.Vote).Where(v => v.Active == true).OrderBy(c => c.Priority).ToListAsync();
            }

            return await _context.Users.Where(c => c.Id == userId)
                    .SelectMany(c => c.UserGroups).Select(c => c.Group)
                    .SelectMany(c => c.VoteGroups).Select(v => v.Vote).OrderBy(c => c.Priority).ToListAsync();
        }

        public async Task<List<Vote>> GetAllUserVote(string userId, bool activeFilter = false)
        {
            return null;
        }

        public async Task<bool> AddVoteAsync(VoteDto voteDto)
        {
            try
            {
                var vote = voteDto.Vote;
                vote.VoteId = Guid.NewGuid();
                vote.UserId = voteDto.UserId;
                vote.Active = true;
                vote.CreatedTime = DateTime.Now;
                vote.FinishTime = voteDto.Vote.FinishTime;


                var res = await _context.Votes.AddAsync(vote, CancellationToken.None);
                var id = res.Entity.VoteId;

                var voteAnswer = voteDto.VoteAnswers;

                foreach (var answer in voteAnswer)
                {
                    answer.VoteAnswerId = new Guid();
                    answer.Vote = res.Entity;
                    answer.VoteId = id;
                    await _context.AddAsync(answer, CancellationToken.None);
                }

                await _context.SaveChangesAsync();

                var requestGroups = voteDto.Groups.Where(r => r.Selected)
                    .Select(u => u.GroupId)
                    .ToList();

                foreach (var requestGroup in requestGroups)
                {
                    await _context.VoteGroups.AddAsync(new VoteGroup()
                    {
                        GroupId = requestGroup,
                        VoteGroupId = new Guid(),
                        VoteId = id,
                    });

                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<VoteCountDto>> GetVoteCount(Guid voteId)
        {
            var option = await _context.VoteAnswers.Where(a => a.VoteId == voteId).ToListAsync();
            var answers = await _context.UserVoteAnswers.Where(a => a.VoteId == voteId).ToListAsync();
            List<VoteCountDto> res = new List<VoteCountDto>();
            foreach (var item in option)
            {
                res.Add(new VoteCountDto()
                {
                    Answer = item.Option,
                    Count = answers.Count(e => e.VoteOptionId == item.VoteAnswerId),
                });
            }

            return res;
        }
    }
}
