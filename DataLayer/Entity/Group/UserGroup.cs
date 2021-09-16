using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using DataLayer.Entity.User;

namespace DataLayer.Entity.Group
{
    public class UserGroup
    {
        [Key]
        public Guid UserGroupId { get; set; }

        #region Relation

        public Guid GroupId { get; set; }
        public string UserId { get; set; }

        [ForeignKey(name: "UserId")]
        public AppUser User { get; set; }

        [ForeignKey(name: "GroupId")]
        public Group Group { get; set; }
        
        #endregion
    }
}
