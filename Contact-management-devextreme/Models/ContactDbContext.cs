using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Contact_management_devextreme.Models;

public partial class ContactDbContext : DbContext
{
    public ContactDbContext()
    {
    }

    public ContactDbContext(DbContextOptions<ContactDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=contactDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__7121FD3567F3BCD3");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Contacts__4849DA012D3BBC9E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Contacts__AB6E616458AB77F0").IsUnique();

            entity.Property(e => e.ContactId).HasColumnName("contactId");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
        });

        modelBuilder.Entity<RefreshTokens>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__RefreshT__F3DBC573BAEC33F8");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.RefreshToken).HasColumnName("refreshToken");
            entity.Property(e => e.TokenId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tokenId");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__DD5D5A42EB7C036C");

            entity.Property(e => e.TaskId).HasColumnName("taskId");
            entity.Property(e => e.AssignedToId).HasColumnName("assigned_toId");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasColumnType("date")
                .HasColumnName("due_date");
            entity.Property(e => e.Priority)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AssignedToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tasks__assigned___4CA06362");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F59DA578D");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC57231CDD028").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
