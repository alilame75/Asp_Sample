using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using DataLayer.Entity.User;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataLayer.Entity.Vote
{
    public class UserVoteAnswer
    {
        [Key]
        public Guid UserVoteAnswerId { get; set; }

        public DateTime SubmitTime { get; set; }


        #region Relation

        public string UserId { get; set; }
        public Guid VoteId { get; set; }
        public Guid VoteOptionId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [ForeignKey("VoteId")]
        public Vote Vote { get; set; }

        [ForeignKey("VoteOptionId")]
        public VoteAnswer Option { get; set; }


        #endregion
    }
}
