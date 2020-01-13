﻿// <auto-generated />
using System;
using Capstone2019.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Capstone2019.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191122193550_OwnerID")]
    partial class OwnerID
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Capstone2019.Models.CompTag", b =>
                {
                    b.Property<int>("CompTag_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Comp_ID");

                    b.Property<int>("Owner_ID");

                    b.Property<string>("Tags");

                    b.HasKey("CompTag_ID");

                    b.ToTable("CompTags");
                });

            modelBuilder.Entity("Capstone2019.Models.Composition", b =>
                {
                    b.Property<int>("Composition_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date_Created");

                    b.Property<DateTime>("Last_Saved");

                    b.Property<string>("Notes")
                        .IsRequired();

                    b.Property<int>("Owner_ID");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Composition_ID");

                    b.ToTable("Compositions");
                });

            modelBuilder.Entity("Capstone2019.Models.Favourite", b =>
                {
                    b.Property<int>("Favourite_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Owner_ID");

                    b.Property<int>("Record_ID");

                    b.HasKey("Favourite_ID");

                    b.ToTable("Favourites");
                });

            modelBuilder.Entity("Capstone2019.Models.RecTag", b =>
                {
                    b.Property<int>("RecTag_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Owner_ID");

                    b.Property<int>("Rec_ID");

                    b.Property<string>("Tags");

                    b.HasKey("RecTag_ID");

                    b.ToTable("RecTags");
                });

            modelBuilder.Entity("Capstone2019.Models.Record", b =>
                {
                    b.Property<int>("Record_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Is_Fav");

                    b.Property<int>("Length");

                    b.Property<string>("Notes_And_Chords")
                        .IsRequired();

                    b.Property<string>("Record_Name")
                        .IsRequired();

                    b.Property<string>("Root")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasConversion(new ValueConverter<string, string>(v => default(string), v => default(string), new ConverterMappingHints(size: 1)));

                    b.HasKey("Record_ID");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("Capstone2019.Models.User", b =>
                {
                    b.Property<int>("User_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email_Address")
                        .IsRequired();

                    b.Property<string>("First_Name");

                    b.Property<bool>("Is_Admin");

                    b.Property<string>("Last_Name");

                    b.Property<DateTime>("Last_Sign_In");

                    b.Property<string>("Profile_Picture");

                    b.Property<string>("User_Name")
                        .IsRequired();

                    b.HasKey("User_ID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
