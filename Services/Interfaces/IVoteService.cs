using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entity.Vote;
using Services.DTOs.Vote;

namespace Services.Interfaces
{
    public interface IVoteService
    {
        Task<List<Vote>> GetAllVote(bool activeFilter = false);
        Task<List<Vote>> GetAllVote(string userId, bool activeFilter = false);
        Task<List<Vote>> GetAllUserVote(string userId, bool activeFilter = false);
        Task<bool> AddVoteAsync(VoteDto voteDto);
        Task<List<VoteCountDto>> GetVoteCount(Guid voteId);
    }
}
