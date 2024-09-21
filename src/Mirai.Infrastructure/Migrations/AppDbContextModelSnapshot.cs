﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mirai.Infrastructure.Common.Persistence;

#nullable disable

namespace Mirai.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mirai.Domain.Organizations.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Mirai.Domain.Projects.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.Retrospective", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Retrospectives");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.RetrospectiveColumn", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RetrospectiveId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("RetrospectiveId");

                    b.ToTable("RetrospectiveColumns");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.RetrospectiveItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RetrospectiveColumnId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Votes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("RetrospectiveColumnId");

                    b.ToTable("RetrospectiveItems");
                });

            modelBuilder.Entity("Mirai.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Mirai.Domain.WikiPages.WikiPage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ParentWikiPageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParentWikiPageId");

                    b.HasIndex("ProjectId");

                    b.ToTable("WikiPages");
                });

            modelBuilder.Entity("Mirai.Domain.WikiPages.WikiPageComment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WikiPageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WikiPageId");

                    b.ToTable("WikiPageComment");
                });

            modelBuilder.Entity("Mirai.Domain.WorkItems.WorkItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssigneeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentWorkItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("ParentWorkItemId");

                    b.HasIndex("ProjectId");

                    b.ToTable("WorkItems");
                });

            modelBuilder.Entity("Mirai.Domain.WorkItems.WorkItemComment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WorkItemId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WorkItemId");

                    b.ToTable("WorkItemComments");
                });

            modelBuilder.Entity("OrganizationUser", b =>
                {
                    b.Property<Guid>("MembersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrganizationsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MembersId", "OrganizationsId");

                    b.HasIndex("OrganizationsId");

                    b.ToTable("OrganizationUser");
                });

            modelBuilder.Entity("Mirai.Domain.Projects.Project", b =>
                {
                    b.HasOne("Mirai.Domain.Organizations.Organization", "Organization")
                        .WithMany("Projects")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.Retrospective", b =>
                {
                    b.HasOne("Mirai.Domain.Projects.Project", "Project")
                        .WithMany("Retrospectives")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.RetrospectiveColumn", b =>
                {
                    b.HasOne("Mirai.Domain.Retrospectives.Retrospective", "Retrospective")
                        .WithMany("Columns")
                        .HasForeignKey("RetrospectiveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Retrospective");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.RetrospectiveItem", b =>
                {
                    b.HasOne("Mirai.Domain.Users.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mirai.Domain.Retrospectives.RetrospectiveColumn", "RetrospectiveColumn")
                        .WithMany("Items")
                        .HasForeignKey("RetrospectiveColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("RetrospectiveColumn");
                });

            modelBuilder.Entity("Mirai.Domain.WikiPages.WikiPage", b =>
                {
                    b.HasOne("Mirai.Domain.WikiPages.WikiPage", "ParentWikiPage")
                        .WithMany("SubWikiPages")
                        .HasForeignKey("ParentWikiPageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Mirai.Domain.Projects.Project", null)
                        .WithMany("WikiPages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentWikiPage");
                });

            modelBuilder.Entity("Mirai.Domain.WikiPages.WikiPageComment", b =>
                {
                    b.HasOne("Mirai.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mirai.Domain.WikiPages.WikiPage", "WikiPage")
                        .WithMany("Comments")
                        .HasForeignKey("WikiPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WikiPage");
                });

            modelBuilder.Entity("Mirai.Domain.WorkItems.WorkItem", b =>
                {
                    b.HasOne("Mirai.Domain.Users.User", "Assignee")
                        .WithMany("WorkItems")
                        .HasForeignKey("AssigneeId");

                    b.HasOne("Mirai.Domain.WorkItems.WorkItem", "ParentWorkItem")
                        .WithMany("ChildWorkItems")
                        .HasForeignKey("ParentWorkItemId");

                    b.HasOne("Mirai.Domain.Projects.Project", "Project")
                        .WithMany("WorkItems")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignee");

                    b.Navigation("ParentWorkItem");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Mirai.Domain.WorkItems.WorkItemComment", b =>
                {
                    b.HasOne("Mirai.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mirai.Domain.WorkItems.WorkItem", "WorkItem")
                        .WithMany("Comments")
                        .HasForeignKey("WorkItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WorkItem");
                });

            modelBuilder.Entity("OrganizationUser", b =>
                {
                    b.HasOne("Mirai.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mirai.Domain.Organizations.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mirai.Domain.Organizations.Organization", b =>
                {
                    b.Navigation("Projects");
                });

            modelBuilder.Entity("Mirai.Domain.Projects.Project", b =>
                {
                    b.Navigation("Retrospectives");

                    b.Navigation("WikiPages");

                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.Retrospective", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("Mirai.Domain.Retrospectives.RetrospectiveColumn", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Mirai.Domain.Users.User", b =>
                {
                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("Mirai.Domain.WikiPages.WikiPage", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("SubWikiPages");
                });

            modelBuilder.Entity("Mirai.Domain.WorkItems.WorkItem", b =>
                {
                    b.Navigation("ChildWorkItems");

                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
