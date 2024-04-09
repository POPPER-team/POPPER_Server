using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace POPPER_Server.Models;

public partial class PopperdbContext : DbContext
{
    private readonly string _connectionString;
    public PopperdbContext(string coneccionString)
    {
        _connectionString = coneccionString;
    }

    public PopperdbContext(DbContextOptions<PopperdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Following> Followings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Following>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Following");

            entity.HasIndex(e => e.FollowingId, "FollowingID");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FollowingId).HasColumnName("FollowingID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.FollowingNavigation).WithMany(p => p.FollowingFollowingNavigations)
                .HasForeignKey(d => d.FollowingId)
                .HasConstraintName("Following_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.FollowingUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Following_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Created).HasColumnType("date");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.Language).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PreferedUnits).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);
            entity.Property(e => e.WebLink).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
