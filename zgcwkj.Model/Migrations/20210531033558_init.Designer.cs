﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using zgcwkj.Model;

namespace zgcwkj.Model.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20210531033558_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("zgcwkj.Model.Models.SysUserModel", b =>
                {
                    b.Property<string>("UserID")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("user_id");

                    b.Property<string>("Password")
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("UserID");

                    b.ToTable("sys_user");

                    b.HasData(
                        new
                        {
                            UserID = "d5916dc8cb46ccc35026a9c54fbf16e7",
                            Password = "Password",
                            UserName = "UserName"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
