﻿// <auto-generated />
using System;
using FriendBook.IdentityServer.API.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FriendBook.IdentityServer.API.DAL.Migrations
{
    [DbContext(typeof(IdentityContext))]
    partial class IdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FriendBook.IdentityServer.API.Domain.Entities.Account", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("pk_account_id");

                    b.Property<string>("Company")
                        .HasColumnType("character varying")
                        .HasColumnName("company");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_date");

                    b.Property<string>("Email")
                        .HasColumnType("character varying")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasColumnType("character varying")
                        .HasColumnName("full_name");

                    b.Property<string>("Info")
                        .HasColumnType("character varying")
                        .HasColumnName("info");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("login");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("password");

                    b.Property<string>("Profession")
                        .HasColumnType("character varying")
                        .HasColumnName("profession");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("salt");

                    b.Property<string>("Telephone")
                        .HasColumnType("character varying")
                        .HasColumnName("telephone");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("accounts", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
