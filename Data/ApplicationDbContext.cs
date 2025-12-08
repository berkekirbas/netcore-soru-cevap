using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Models;

namespace SoruCevapPortal.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<QuestionVote> QuestionVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UserRole composite key
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // UserRole relationships
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Question relationships
        modelBuilder.Entity<Question>()
            .HasOne(q => q.User)
            .WithMany(u => u.Questions)
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Category)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Answer relationships
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.User)
            .WithMany(u => u.Answers)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // QuestionVote relationships
        modelBuilder.Entity<QuestionVote>()
            .HasOne(qv => qv.Question)
            .WithMany(q => q.QuestionVotes)
            .HasForeignKey(qv => qv.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionVote>()
            .HasOne(qv => qv.User)
            .WithMany(u => u.QuestionVotes)
            .HasForeignKey(qv => qv.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint for QuestionVote (one vote per user per question)
        modelBuilder.Entity<QuestionVote>()
            .HasIndex(qv => new { qv.QuestionId, qv.UserId })
            .IsUnique();
    }
}

