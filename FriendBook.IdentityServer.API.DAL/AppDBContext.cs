﻿using FriendBook.IdentityServer.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FriendBook.IdentityServer.API.DAL
{
    public partial class IdentityContext : DbContext
    {
        public const string NameConnection = "NpgConnectionString";
        public DbSet<Account> Accounts { get; set; }

        public void UpdateDatabase()
        {
            Database.Migrate();
        }
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
