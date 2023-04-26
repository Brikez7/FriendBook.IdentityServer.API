using FriendBook.IdentityServer.API.DAL.Configuration.DataType;
using FriendBook.IdentityServer.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendBook.IdentityServer.API.DAL.Configuration
{
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public const string Table_name = "accounts";

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable(Table_name);

            builder.HasKey(e => new { e.Id });

            builder.HasIndex(e => e.Login)
                   .IsUnique();

            builder.Property(e => e.Id)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("pk_account_id");

            builder.Property(e => e.Profession)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("profession");

            builder.Property(e => e.Info)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("info");

            builder.Property(e => e.FullName)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("full_name");

            builder.Property(e => e.Telephone)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("telephone");

            builder.Property(e => e.Email)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("email");

            builder.Property(e => e.Company)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("company");

            builder.Property(e => e.Login)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("login");

            builder.Property(e => e.Password)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("password");

            builder.Property(e => e.Salt)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("salt");

            builder.Property(e => e.CreateDate)
                   .HasColumnName("create_date");

        }
    }
}
