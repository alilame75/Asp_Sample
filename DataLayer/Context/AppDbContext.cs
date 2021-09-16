using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Entity.Group;
using DataLayer.Entity.User;
using DataLayer.Entity.Vote;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {

        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteAnswer> VoteAnswers { get; set; }
        public DbSet<UserVoteAnswer> UserVoteAnswers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<VoteGroup> VoteGroups { get; set; }


        public AppDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // call the base if you are using Identity service.
            // Important
            base.OnModelCreating(builder);
            
         
            

            // Code here ...
        }
    }
}
