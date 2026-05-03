using System;
using ImperiumAO.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ImperiumAO.Server.Data.Migrations
{
    [DbContext(typeof(ImperiumAOContext))]
    partial class ImperiumAOContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ImperiumAO.Common.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("ImperiumAO.Common.Entities.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Map")
                        .HasColumnType("int");

                    b.Property<int>("MaxHealth")
                        .HasColumnType("int");

                    b.Property<int>("MaxMana")
                        .HasColumnType("int");

                    b.Property<int>("MaxStamina")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<int>("Mana")
                        .HasColumnType("int");

                    b.Property<int>("Stamina")
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Character");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Character");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("ImperiumAO.Common.Entities.Npc", b =>
                {
                    b.HasBaseType("ImperiumAO.Common.Entities.Character");

                    b.Property<int>("NpcTypeId")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("Npc");
                });

            modelBuilder.Entity("ImperiumAO.Common.Entities.Player", b =>
                {
                    b.HasBaseType("ImperiumAO.Common.Entities.Character");

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<int?>("Bank")
                        .HasColumnType("int");

                    b.Property<int?>("Charisma")
                        .HasColumnType("int");

                    b.Property<int?>("Class")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Constitution")
                        .HasColumnType("int");

                    b.Property<int?>("Dexterity")
                        .HasColumnType("int");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<int?>("Gold")
                        .HasColumnType("int");

                    b.Property<int?>("Intelligence")
                        .HasColumnType("int");

                    b.Property<int?>("Race")
                        .HasColumnType("int");

                    b.Property<int?>("Strength")
                        .HasColumnType("int");

                    b.Property<int?>("Wisdom")
                        .HasColumnType("int");

                    b.HasOne("ImperiumAO.Common.Entities.Account", "Account")
                        .WithMany("Players")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasDiscriminator().HasValue("Player");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("ImperiumAO.Common.Entities.Account", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
