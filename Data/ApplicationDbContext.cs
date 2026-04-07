using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Database Tabloları
        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Branch - Employee İlişkisi
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict); // Şube silinirse çalışanlar kalır (başka şubeye atanabilir)

            // Branch - Customer İlişkisi
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Branch)
                .WithMany(b => b.Customers)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict); // Şube silinirse müşteriler kalır

            // Employee - Customer İlişkisi (Müşteri Temsilcisi)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Representative)
                .WithMany(e => e.Customers)
                .HasForeignKey(c => c.RepresentativeId)
                .OnDelete(DeleteBehavior.Restrict); // Çalışan silinirse müşteriler kalır (başka temsilciye atanabilir)

            // Customer - ContactMessage İlişkisi (Opsiyonel)
            modelBuilder.Entity<ContactMessage>()
                .HasOne(cm => cm.Customer)
                .WithMany(c => c.ContactMessages)
                .HasForeignKey(cm => cm.CustomerId)
                .OnDelete(DeleteBehavior.SetNull); // Müşteri silinirse mesajlar kalır ama CustomerId null olur

            // User için Index (Username unique olmalı)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Employee için Index (TC unique olmalı)
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.TC)
                .IsUnique();

            // Customer için Index (TC unique olmalı)
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.TC)
                .IsUnique();

            // Customer için Index (AccountNumber unique olmalı)
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AccountNumber)
                .IsUnique();
        }
    }
}



