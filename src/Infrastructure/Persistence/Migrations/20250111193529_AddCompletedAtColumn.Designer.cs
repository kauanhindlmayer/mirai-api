﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250111193529_AddCompletedAtColumn")]
    partial class AddCompletedAtColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "vector");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Boards.Board", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("Domain.Boards.BoardCard", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("BoardColumnId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("WorkItemId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BoardColumnId");

                    b.HasIndex("WorkItemId");

                    b.ToTable("BoardCards");
                });

            modelBuilder.Entity("Domain.Boards.BoardColumn", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("BoardId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DefinitionOfDone")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("WipLimit")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("BoardColumns");
                });

            modelBuilder.Entity("Domain.Organizations.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Domain.Projects.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Domain.Retrospectives.Retrospective", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Retrospectives");
                });

            modelBuilder.Entity("Domain.Retrospectives.RetrospectiveColumn", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<Guid>("RetrospectiveId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("RetrospectiveId");

                    b.ToTable("RetrospectiveColumns");
                });

            modelBuilder.Entity("Domain.Retrospectives.RetrospectiveItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<Guid>("RetrospectiveColumnId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Votes")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("RetrospectiveColumnId");

                    b.ToTable("RetrospectiveItems");
                });

            modelBuilder.Entity("Domain.Tags.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Domain.Teams.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("IdentityId")
                        .IsUnique();

                    b.HasIndex("TeamId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ParentWikiPageId")
                        .HasColumnType("uuid");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ParentWikiPageId");

                    b.HasIndex("ProjectId");

                    b.ToTable("WikiPages");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPageComment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("WikiPageId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("WikiPageId");

                    b.ToTable("WikiPageComments");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPageView", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ViewedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ViewerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WikiPageId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WikiPageId");

                    b.ToTable("WikiPageViews");
                });

            modelBuilder.Entity("Domain.WorkItems.WorkItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AcceptanceCriteria")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("AssigneeId")
                        .HasColumnType("uuid");

                    b.Property<int>("Code")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentWorkItemId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<Vector>("SearchVector")
                        .HasColumnType("vector");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.ComplexProperty<Dictionary<string, object>>("Classification", "Domain.WorkItems.WorkItem.Classification#Classification", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("ValueArea")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Planning", "Domain.WorkItems.WorkItem.Planning#Planning", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int?>("Priority")
                                .HasColumnType("integer");

                            b1.Property<int?>("StoryPoints")
                                .HasColumnType("integer");
                        });

                    b.HasKey("Id");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("ParentWorkItemId");

                    b.HasIndex("ProjectId", "Code")
                        .IsUnique();

                    b.ToTable("WorkItems");
                });

            modelBuilder.Entity("Domain.WorkItems.WorkItemComment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WorkItemId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WorkItemId");

                    b.ToTable("WorkItemComments");
                });

            modelBuilder.Entity("OrganizationUser", b =>
                {
                    b.Property<Guid>("MembersId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("OrganizationsId")
                        .HasColumnType("uuid");

                    b.HasKey("MembersId", "OrganizationsId");

                    b.HasIndex("OrganizationsId");

                    b.ToTable("OrganizationUser");
                });

            modelBuilder.Entity("TagWorkItem", b =>
                {
                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WorkItemsId")
                        .HasColumnType("uuid");

                    b.HasKey("TagsId", "WorkItemsId");

                    b.HasIndex("WorkItemsId");

                    b.ToTable("WorkItemTag", (string)null);
                });

            modelBuilder.Entity("Domain.Boards.Board", b =>
                {
                    b.HasOne("Domain.Projects.Project", "Project")
                        .WithMany("Boards")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.Boards.BoardCard", b =>
                {
                    b.HasOne("Domain.Boards.BoardColumn", "BoardColumn")
                        .WithMany("Cards")
                        .HasForeignKey("BoardColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.WorkItems.WorkItem", "WorkItem")
                        .WithMany()
                        .HasForeignKey("WorkItemId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("BoardColumn");

                    b.Navigation("WorkItem");
                });

            modelBuilder.Entity("Domain.Boards.BoardColumn", b =>
                {
                    b.HasOne("Domain.Boards.Board", "Board")
                        .WithMany("Columns")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("Domain.Projects.Project", b =>
                {
                    b.HasOne("Domain.Organizations.Organization", "Organization")
                        .WithMany("Projects")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Domain.Retrospectives.Retrospective", b =>
                {
                    b.HasOne("Domain.Teams.Team", "Team")
                        .WithMany("Retrospectives")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Domain.Retrospectives.RetrospectiveColumn", b =>
                {
                    b.HasOne("Domain.Retrospectives.Retrospective", "Retrospective")
                        .WithMany("Columns")
                        .HasForeignKey("RetrospectiveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Retrospective");
                });

            modelBuilder.Entity("Domain.Retrospectives.RetrospectiveItem", b =>
                {
                    b.HasOne("Domain.Users.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Retrospectives.RetrospectiveColumn", "RetrospectiveColumn")
                        .WithMany("Items")
                        .HasForeignKey("RetrospectiveColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("RetrospectiveColumn");
                });

            modelBuilder.Entity("Domain.Tags.Tag", b =>
                {
                    b.HasOne("Domain.Projects.Project", "Project")
                        .WithMany("Tags")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.Teams.Team", b =>
                {
                    b.HasOne("Domain.Projects.Project", "Project")
                        .WithMany("Teams")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.HasOne("Domain.Teams.Team", null)
                        .WithMany("Members")
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPage", b =>
                {
                    b.HasOne("Domain.Users.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.WikiPages.WikiPage", "ParentWikiPage")
                        .WithMany("SubWikiPages")
                        .HasForeignKey("ParentWikiPageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Projects.Project", null)
                        .WithMany("WikiPages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("ParentWikiPage");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPageComment", b =>
                {
                    b.HasOne("Domain.Users.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.WikiPages.WikiPage", "WikiPage")
                        .WithMany("Comments")
                        .HasForeignKey("WikiPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("WikiPage");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPageView", b =>
                {
                    b.HasOne("Domain.WikiPages.WikiPage", "WikiPage")
                        .WithMany("Views")
                        .HasForeignKey("WikiPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WikiPage");
                });

            modelBuilder.Entity("Domain.WorkItems.WorkItem", b =>
                {
                    b.HasOne("Domain.Users.User", "Assignee")
                        .WithMany("WorkItems")
                        .HasForeignKey("AssigneeId");

                    b.HasOne("Domain.WorkItems.WorkItem", "ParentWorkItem")
                        .WithMany("ChildWorkItems")
                        .HasForeignKey("ParentWorkItemId");

                    b.HasOne("Domain.Projects.Project", "Project")
                        .WithMany("WorkItems")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignee");

                    b.Navigation("ParentWorkItem");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.WorkItems.WorkItemComment", b =>
                {
                    b.HasOne("Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.WorkItems.WorkItem", "WorkItem")
                        .WithMany("Comments")
                        .HasForeignKey("WorkItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WorkItem");
                });

            modelBuilder.Entity("OrganizationUser", b =>
                {
                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Organizations.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TagWorkItem", b =>
                {
                    b.HasOne("Domain.Tags.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.WorkItems.WorkItem", null)
                        .WithMany()
                        .HasForeignKey("WorkItemsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Boards.Board", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("Domain.Boards.BoardColumn", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("Domain.Organizations.Organization", b =>
                {
                    b.Navigation("Projects");
                });

            modelBuilder.Entity("Domain.Projects.Project", b =>
                {
                    b.Navigation("Boards");

                    b.Navigation("Tags");

                    b.Navigation("Teams");

                    b.Navigation("WikiPages");

                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("Domain.Retrospectives.Retrospective", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("Domain.Retrospectives.RetrospectiveColumn", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Domain.Teams.Team", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Retrospectives");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("Domain.WikiPages.WikiPage", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("SubWikiPages");

                    b.Navigation("Views");
                });

            modelBuilder.Entity("Domain.WorkItems.WorkItem", b =>
                {
                    b.Navigation("ChildWorkItems");

                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
