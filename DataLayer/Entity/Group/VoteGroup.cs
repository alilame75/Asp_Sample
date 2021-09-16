using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Entity.Group
{
    public class VoteGroup
    {
        [Key]
        public Guid VoteGroupId { get; set; }

        #region Relation

        public Guid GroupId { get; set; }
        public Guid VoteId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        [ForeignKey("VoteId")]
        public Vote.Vote Vote { get; set; }

        #endregion
    }
}
