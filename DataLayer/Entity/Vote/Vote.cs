using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using DataLayer.Entity.Group;
using DataLayer.Entity.User;

namespace DataLayer.Entity.Vote
{
    public class Vote
    {
        [Key]
        public Guid VoteId { get; set; }

        [MaxLength(150)]
        public string VoteName { get; set; }

        public string VoteDescription { get; set; }

        public string Question { get; set; }

        public bool Active { get; set; }
        public int UserHowManyCanVote { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int Priority { get; set; }



        #region Relation

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser CreatedUser { get; set; }

        public List<VoteAnswer> Options { get; set; }
        public List<UserVoteAnswer> UserVoteAnswers { get; set; }
        public List<VoteGroup> VoteGroups { get; set; }


        #endregion

    }
}
