using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Entity.Group;
using DataLayer.Entity.Vote;

namespace Services.DTOs.Vote
{
    public class VoteDto
    {
        public DataLayer.Entity.Vote.Vote Vote { get; set; }
        public List<VoteAnswer> VoteAnswers { get; set; }
        public string UserId { get; set; }
        public List<GroupSelectForVote> Groups { get; set; } 
    }

    public class GroupSelectForVote
    {
        public bool Selected { get; set; }
        public Guid GroupId { get; set; }
        public string GroupTitle { get; set; }
    }

}
