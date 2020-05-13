using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FarukMC.Areas.Identity.Data;
using FarukMC.Models;

namespace FarukMC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<FarukMC.Models.SurgeryRoom> SurgeryRoom { get; set; }
        public DbSet<FarukMC.Models.Item> Item { get; set; }
        public DbSet<FarukMC.Models.AnesthesiaTechnique> AnesthesiaTechnique { get; set; }
        public DbSet<FarukMC.Models.SurgicalDepartment> SurgicalDepartment { get; set; }
        public DbSet<FarukMC.Models.ItemsBooked> ItemsBooked { get; set; }
        public DbSet<FarukMC.Models.Anesthetics> Anesthetics { get; set; }
        public DbSet<FarukMC.Models.SMS> SMS { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ItemsBooked>().HasIndex(c => new { c.BookingId, c.ItemId }).IsUnique();
            modelBuilder.Entity<ItemsBooked>().HasIndex(c => new { c.BookingId });

        }


        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var currentUsername = !string.IsNullOrEmpty(userId)
                ? userId
                : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedDate = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).CreatedBy = currentUsername;
                }
                if (entity.State == EntityState.Modified)
                {
                    Entry((BaseEntity)entity.Entity).Property(x => x.CreatedDate).IsModified = false;
                    Entry((BaseEntity)entity.Entity).Property(x => x.CreatedBy).IsModified = false;
                }

                ((BaseEntity)entity.Entity).ModifiedDate = DateTime.UtcNow;
                ((BaseEntity)entity.Entity).ModifiedBy = currentUsername;
            }
        }


    }
}