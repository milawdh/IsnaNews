using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class IsnaNewsContext : DbContext
{
    public IsnaNewsContext()
    {
    }

    public IsnaNewsContext(DbContextOptions<IsnaNewsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAboutUs> TblAboutUs { get; set; }

    public virtual DbSet<TblAdvertisement> TblAdvertisement { get; set; }

    public virtual DbSet<TblCategory> TblCategory { get; set; }

    public virtual DbSet<TblContactUs> TblContactUs { get; set; }

    public virtual DbSet<TblImage> TblImage { get; set; }

    public virtual DbSet<TblKeyWord> TblKeyWord { get; set; }

    public virtual DbSet<TblNews> TblNews { get; set; }

    public virtual DbSet<TblNewsComment> TblNewsComment { get; set; }

    public virtual DbSet<TblNewsImageRel> TblNewsImageRel { get; set; }

    public virtual DbSet<TblNewsKeyWordRel> TblNewsKeyWordRel { get; set; }

    public virtual DbSet<TblNewsVideoRel> TblNewsVideoRel { get; set; }

    public virtual DbSet<TblRole> TblRole { get; set; }

    public virtual DbSet<TblRolePermissions> TblRolePermissions { get; set; }

    public virtual DbSet<TblRoleRolePermissionsRel> TblRoleRolePermissionsRel { get; set; }

    public virtual DbSet<TblUser> TblUser { get; set; }

    public virtual DbSet<TblVideo> TblVideo { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=IsnaNews;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAdvertisement>(entity =>
        {
            entity.HasOne(d => d.MainBaner).WithMany(p => p.TblAdvertisement).HasConstraintName("FK_TblAdvertisement_TblImage");
        });

        modelBuilder.Entity<TblCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TblCategoryId");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_TblCategoryId_TblCategoryId");
        });

        modelBuilder.Entity<TblNews>(entity =>
        {
            entity.Property(e => e.DatePosted).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsImportantNews).HasComment("If it is 1 Will show on Index Page Top carousel\r\n");

            entity.HasOne(d => d.Category).WithMany(p => p.TblNews).HasConstraintName("FK_TblNews_TblCategoryId");

            entity.HasOne(d => d.MainImage).WithMany(p => p.TblNews).IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TblNews_TblImage");

            entity.HasOne(d => d.Reporter).WithMany(p => p.TblNews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TblNews_TblUser");
        });

        modelBuilder.Entity<TblNewsComment>(entity =>
        {
            entity.Property(e => e.DatePosted).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_TblNewsComment_TblNewsComment");

            entity.HasOne(d => d.Post).WithMany(p => p.TblNewsComment).HasConstraintName("FK_TblNewsComment_TblNews");

            entity.HasOne(d => d.User).WithMany(p => p.TblNewsComment).HasConstraintName("FK_TblNewsComment_TblUser");
        });

        modelBuilder.Entity<TblNewsImageRel>(entity =>
        {
            entity.HasOne(d => d.Image).WithMany(p => p.TblNewsImageRel).HasConstraintName("FK_TblNewsImageRel_TblImage");

            entity.HasOne(d => d.News).WithMany(p => p.TblNewsImageRel).HasConstraintName("FK_TblNewsImageRel_TblNews");
        });

        modelBuilder.Entity<TblNewsKeyWordRel>(entity =>
        {
            entity.HasOne(d => d.KeyWord).WithMany(p => p.TblNewsKeyWordRel).HasConstraintName("FK_TblNewsKeyWordRel_TblKeyWord");

            entity.HasOne(d => d.News).WithMany(p => p.TblNewsKeyWordRel).HasConstraintName("FK_TblNewsKeyWordRel_TblNews");
        });

        modelBuilder.Entity<TblNewsVideoRel>(entity =>
        {
            entity.HasOne(d => d.News).WithMany(p => p.TblNewsVideoRel).HasConstraintName("FK_TblNewsVideoRel_TblNews");

            entity.HasOne(d => d.Video).WithMany(p => p.TblNewsVideoRel).HasConstraintName("FK_TblNewsVideoRel_TblVideo");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(entity => entity.Id).HasName("PK_TblRole");
            entity.ToTable(tb => tb.HasComment("0 Comment Blog\r\n1 Add News \r\n2 Edit News-Blogs-Comments\r\n3 Users"));
        });
        modelBuilder.Entity<TblRolePermissions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TblRoleClaims");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_TblRolePermissions_TblRolePermissions");
        });

        modelBuilder.Entity<TblRoleRolePermissionsRel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TblRoleClaimsRoleRel");

            entity.HasOne(d => d.Permission).WithMany(p => p.TblRoleRolePermissionsRel).HasConstraintName("FK_TblRole-RolePermissionsRel_TblRolePermissions");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRoleRolePermissionsRel).HasConstraintName("FK_TblRole-RolePermissionsRel_TblRole");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.Property(e => e.DateSigned).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastLoginDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProfileImageId).HasDefaultValue(1);
            entity.Property(e => e.RoleId).HasDefaultValue(1);

            entity.HasOne(d => d.ProfileImage).WithMany(p => p.TblUser)
                .HasConstraintName("FK_TblUser_TblImage").IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Role).WithMany(p => p.TblUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TblUser_TblRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
