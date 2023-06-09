﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using e_Movies_Platform.Models;

namespace e_Movies_Platform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<e_Movies_Platform.Models.Genre> Genre { get; set; }
        public DbSet<e_Movies_Platform.Models.CastCrewRole> CastCrewRole { get; set; }
        public DbSet<e_Movies_Platform.Models.CastCrew> CastCrew { get; set; }
        public DbSet<e_Movies_Platform.Models.Movie> Movie { get; set; }
        public DbSet<e_Movies_Platform.Models.Subscription> Subscription { get; set; }
        public DbSet<e_Movies_Platform.Models.WishList> WishList { get; set; }
    }
}