using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.Entity.Group
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }

        public string GroupTitle { get; set; }

        #region Relation

        public List<UserGroup> UserGroups { get; set; }
        public List<VoteGroup> VoteGroups { get; set; } 

        #endregion

    }
}
