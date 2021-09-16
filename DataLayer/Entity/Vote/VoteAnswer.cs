using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entity.Vote
{
    public class VoteAnswer
    {
        [Key]
        public Guid VoteAnswerId { get; set; }

        public string Option { get; set; }

        public string OptionDescription { get; set; }



        #region Relation

        public Guid VoteId { get; set; }

        [ForeignKey("VoteId")]
        public Vote Vote { get; set; }

        public List<UserVoteAnswer> UserVoteAnswers { get; set; }

        #endregion

    }
}
