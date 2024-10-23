
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using FinanceManagement.Models;
using FinanceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CashFlow> CashFlows { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<AccountMaster> AccountMaster { get; set; }
        public DbSet<Expenses> expenses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Upad> Upads { get; set; }
        public DbSet<Other> Others { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DailyBeltUpdate> dailyBeltUpdates { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.SeedUsers(modelBuilder);
            this.SeedRoles(modelBuilder);
            this.SeedUserRoles(modelBuilder);

            

            modelBuilder.Entity<Payment>()
        .HasOne(p => p.CashFlow)
        .WithMany(c => c.Payments)
        .HasForeignKey(e => e.CashFlowId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expenses>()
        .HasOne(e => e.CashFlow) // Foreign key to CashFlow
        .WithMany(c => c.Expenses) // Navigation property for CashFlow
        .HasForeignKey(e => e.CashFlowId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Upad>()
        .HasOne(u => u.CashFlow)
        .WithMany(c => c.Upads)
        .HasForeignKey(e => e.CashFlowId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Other>()
        .HasOne(u => u.CashFlow)
        .WithMany(c => c.Others)
        .HasForeignKey(e => e.CashFlowId)
        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Expenses>()
        .HasOne(e => e.AccountMaster) // Foreign key to CashFlow
        .WithMany(c => c.Expenses) // Navigation property for CashFlow
        .HasForeignKey(e => e.AccountMasterId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Other>()
        .HasOne(e => e.AccountMaster) // Foreign key to CashFlow
        .WithMany(c => c.Others) // Navigation property for CashFlow
        .HasForeignKey(e => e.AccountMasterId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
        .HasOne(p => p.AccountMaster)
        .WithMany(c => c.Payments)
        .HasForeignKey(e => e.AccountMasterId)
        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Upad>()
        .HasOne(u => u.AccountMaster)
        .WithMany(c => c.Upads)
        .HasForeignKey(e => e.AccountMasterId)
        .OnDelete(DeleteBehavior.Cascade);

        }
        

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                Email = "superadmin123@gmail.com",
                EmailConfirmed = true,
                NameOfUser = "SuperAdmin",
                PhoneNumber = "6351507477",
                UserName = "superadmin123@gmail.com",
                LockoutEnabled = true,
                NormalizedEmail = "superadmin123@gmail.com".ToUpper(),
                NormalizedUserName = "superadmin123@gmail.com".ToUpper(),
                BranchCode = 0,
                CreatedBy = "8bf07921-c894-4fb5-b08a-866dc1967bd7",
                CreatedTer = "",
                CompanyName = "Imperial Technologies"

            };



            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "admin123");


            modelBuilder.Entity<ApplicationUser>().HasData(user);
        }




        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "3a021ce2-48ea-4ad6-99a6-e25be387503a", Name = "SuperAdmin", ConcurrencyStamp = "2", NormalizedName = "SuperAdmin".ToUpper() },
                new IdentityRole() { Id = "3a021ce2-48ea-4ad6-99a6-e25be387523a", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin".ToUpper() },
                new IdentityRole() { Id = "3o021ce2-48eo-4od6-99o6-e25be787553o", Name = "Manager", ConcurrencyStamp = "3", NormalizedName = "Manager".ToUpper() }
                
            );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "3a021ce2-48ea-4ad6-99a6-e25be387503a", UserId = "8bf07921-c894-4fb5-b08a-866dc1967bd7" } // SuperAdmin user ID
            );
        }

    }
}

