using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WordWeaver.Data.Entity;

namespace WordWeaver.Data;

public partial class WordWeaverContext : DbContext
{
    public WordWeaverContext()
    {
    }

    public WordWeaverContext(DbContextOptions<WordWeaverContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<CloudFile> CloudFiles { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<Error> Errors { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<React> Reacts { get; set; }

    public virtual DbSet<ReactEnum> ReactEnums { get; set; }

    public virtual DbSet<RoleEnum> RoleEnums { get; set; }

    public virtual DbSet<Social> Socials { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagEnum> TagEnums { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<View> Views { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.ToTable("Bookmarks", "user", tb => tb.HasTrigger("trgBookmarksSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bookmarks_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bookmarks_Users");
        });

        modelBuilder.Entity<CloudFile>(entity =>
        {
            entity.HasKey(e => e.FileId);

            entity.ToTable("CloudFiles", "core", tb =>
                {
                    tb.HasComment("");
                    tb.HasTrigger("trgCloudFilesSetUpdateDatetime");
                });

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(NULL)")
                .HasComment("Uploaded By");

            entity.HasOne(d => d.User).WithMany(p => p.CloudFiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_CloudFiles_Users");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments", "blog", tb => tb.HasTrigger("trgCommentsSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ParentId).HasDefaultValue(0L);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Comments_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.ToTable("Emails", "log");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailTo).HasMaxLength(255);
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        modelBuilder.Entity<Error>(entity =>
        {
            entity.ToTable("Errors", "log");

            entity.Property(e => e.HttpMethod).HasMaxLength(50);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Errors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Errors_Users");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK_Tokens");

            entity.ToTable("Logins", "auth", tb => tb.HasTrigger("trgTokensSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt)
                .HasDefaultValueSql("(dateadd(day,(1),getutcdate()))")
                .HasColumnType("datetime");
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Tokens_Users");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.ToTable("Otps", "auth");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt)
                .HasDefaultValueSql("(dateadd(day,(1),getutcdate()))")
                .HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.OtpValue).HasMaxLength(50);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK_Blogs");

            entity.ToTable("Posts", "blog", tb =>
                {
                    tb.HasComment("");
                    tb.HasTrigger("trgPostsSetUpdateDatetime");
                });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FileIds)
                .HasMaxLength(255)
                .HasComment("Medias");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Posts_Users");
        });

        modelBuilder.Entity<React>(entity =>
        {
            entity.HasKey(e => e.ReactId).HasName("PK_BlogReacts");

            entity.ToTable("Reacts", "blog", tb => tb.HasTrigger("trgReactsSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Blog).WithMany(p => p.Reacts)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogReacts_Blogs");

            entity.HasOne(d => d.ReactEnum).WithMany(p => p.Reacts)
                .HasForeignKey(d => d.ReactEnumId)
                .HasConstraintName("FK_BlogReacts_Reacts");

            entity.HasOne(d => d.User).WithMany(p => p.Reacts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogReacts_Users");
        });

        modelBuilder.Entity<ReactEnum>(entity =>
        {
            entity.HasKey(e => e.ReactEnumId).HasName("PK_Reacts");

            entity.ToTable("ReactEnums", "enum", tb => tb.HasTrigger("trgReactEnumsSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReactName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<RoleEnum>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Roles");

            entity.ToTable("RoleEnums", "enum");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Social>(entity =>
        {
            entity.HasKey(e => e.SocialId).HasName("PK_SocialMedia");

            entity.ToTable("Socials", "user", tb => tb.HasTrigger("trgSocialsSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SocialName).HasMaxLength(50);
            entity.Property(e => e.SocialUrl).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Socials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SocialMedia_Users");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK_BlogTags");

            entity.ToTable("Tags", "blog", tb => tb.HasTrigger("trgTagsSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Blog).WithMany(p => p.Tags)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogTags_BlogTags");

            entity.HasOne(d => d.TagEnum).WithMany(p => p.Tags)
                .HasForeignKey(d => d.TagEnumId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogTags_Tags");
        });

        modelBuilder.Entity<TagEnum>(entity =>
        {
            entity.HasKey(e => e.TagEnumId).HasName("PK_Tags");

            entity.ToTable("TagEnums", "enum", tb => tb.HasTrigger("trgTagEnumsSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TagName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "auth", tb => tb.HasTrigger("trgUsersSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK_Profiles");

            entity.ToTable("UserDetails", "user", tb => tb.HasTrigger("trgUserDetailsSetUpdateDatetime"));

            entity.Property(e => e.AvatarFileId).HasComment("Fk from Cloudfiles");
            entity.Property(e => e.Bio).HasMaxLength(255);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.Website).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Profiles_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles", "auth", tb => tb.HasTrigger("trgUserRolesSetUpdateDatetime"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<View>(entity =>
        {
            entity.HasKey(e => e.BlogViewId).HasName("PK_BlogViews");

            entity.ToTable("Views", "blog");

            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Views)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogViews_Blogs");

            entity.HasOne(d => d.User).WithMany(p => p.Views)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogViews_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
