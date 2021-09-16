using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Entity.Group;
using DataLayer.Entity.Vote;
using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entity.User
{
    public class AppUser : IdentityUser
    {
        public bool Active { get; set; }
        public DateTime RegistrationTime { get; set; }

        #region Relation

        public List<UserGroup> UserGroups { get; set; }
        public List<Vote.Vote> Votes { get; set; }
        public List<UserVoteAnswer> UserVoteAnswers { get; set; }

        #endregion
    }
}
